using CatalogService.Gateway.Aggregators;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Logging;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Cache.CacheManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using NSwag;
using NSwag.Generation.Processors.Security;
using NSwag.AspNetCore;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace CatalogService.Gateway
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var authenticationProviderKey = "ManagerOnly";
            var apiName = Configuration.GetValue<string>("ApiName");
            var apiSecret = Configuration.GetValue<string>("ApiSecret");
            var identityAuthority = Configuration.GetValue<string>("IdentityAuthority");

            IdentityModelEventSource.ShowPII = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            services.AddControllers();

            services.AddAuthentication()
                .AddIdentityServerAuthentication(authenticationProviderKey, options =>
                {
                    options.Authority = identityAuthority;
                    options.RequireHttpsMetadata = false;
                    options.ApiName = apiName;
                    options.ApiSecret = apiSecret;
                    options.SaveToken = true;
                    options.EnableCaching = true;
                });

            services.AddOcelot()
                   .AddCacheManager(x =>
                   {
                       x.WithDictionaryHandle();
                   })
                 .AddTransientDefinedAggregator<CatalogItemsAggregator>();

            services.AddSwaggerDocument(x =>
            {                
                x.AddSecurity("oauth2", new NSwag.OpenApiSecurityScheme
                {
                    Type = OpenApiSecuritySchemeType.OAuth2,
                    Flow = OpenApiOAuth2Flow.Password,
                    TokenUrl = @$"{identityAuthority}/connect/token",
                    AuthorizationUrl = @$"{identityAuthority}/connect/authorize",
                    Flows = new OpenApiOAuthFlows
                    {
                        Password = new NSwag.OpenApiOAuthFlow
                        {
                            AuthorizationUrl = @$"{identityAuthority}/connect/authorize",
                            TokenUrl = @$"{identityAuthority}/connect/token",
                            Scopes = new Dictionary<string, string> { { "catalog", "catalog" }, { "role", "role" }, { "openid", "openid" } }
                        }
                    }

                });
                x.OperationProcessors.Add(new OperationSecurityScopeProcessor("oauth2"));

            });
            services.AddApplicationInsightsTelemetry();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            var apiName = Configuration.GetValue<string>("ApiName");
            var apiSecret = Configuration.GetValue<string>("ApiSecret");


            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/internal/swagger.json", async context =>
                {
                    await context.Response.WriteAsync(await File.ReadAllTextAsync("manual_swagger.json"));
                });

                endpoints.MapControllers();
            });

            app.UseOpenApi();
            app.UseSwaggerUi3(options =>
            {
                options.DocumentPath = "/internal/swagger.json";                
                options.OAuth2Client = new OAuth2ClientSettings
                {
                    ClientId = apiName,
                    ClientSecret = apiSecret,
                    AppName = apiName,
                    UsePkceWithAuthorizationCodeGrant = false,
                    Scopes = { "catalog", "profile", "role" }
                };
            });

            app.UseOcelot();


        }
    }
}

using System.Collections.Generic;
using System.Net;
using Azure.Messaging.ServiceBus;
using CatalogService.BLL.Interfaces;
using CatalogService.BLL.Services;
using CatalogService.Common.MapperProfiles;
using CatalogService.DAL;
using CatalogService.DAL.Interfaces;
using CatalogService.Model;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Logging;
using NSwag;
using NSwag.AspNetCore;
using NSwag.Generation.Processors.Security;
using RiskFirst.Hateoas;
using OpenApiOAuthFlows = NSwag.OpenApiOAuthFlows;

namespace CatalogService.API
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
            IdentityModelEventSource.ShowPII = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var apiName = Configuration.GetValue<string>("ApiName");
            var apiSecret = Configuration.GetValue<string>("ApiSecret");
            var identityAuthority = Configuration.GetValue<string>("IdentityAuthority");

            services.AddControllers(options =>
            {
                options.Filters.Add(new AuthorizeFilter());
            })
            .AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.MaxDepth = 3;
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            });

            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
           .AddIdentityServerAuthentication(options =>
           {
               options.Authority = identityAuthority;
               options.RequireHttpsMetadata = false;
               options.ApiName = apiName;
               options.ApiSecret = apiSecret;
               options.SaveToken = true;
               options.EnableCaching = true;
           });


            services.AddHttpClient();
            services.AddAuthorization();

            var connectionString = Configuration.GetConnectionString("CatalogDbContext");
            services.AddDbContext<CatalogDbContext>(options => options.UseSqlServer(connectionString));

            services.AddScoped<IItemService, ItemService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IMessagingService, MessagingService>();
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            var messageBusConnectionString = Configuration.GetConnectionString("ServiceBus");
            var clientOptions = new ServiceBusClientOptions()
            {
                TransportType = ServiceBusTransportType.AmqpWebSockets
            };

            services.AddSingleton(typeof(ServiceBusClient), new ServiceBusClient(messageBusConnectionString, clientOptions));
            services.AddAutoMapper(typeof(Startup), typeof(CatalogProfile));

            services.AddCors(x =>
            {
                x.AddPolicy("default", policy =>
                {
                    policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin();
                });
            });
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

            services.AddLinks(config =>
            {
                config.AddPolicy<Category>(policy =>
                {
                    policy.RequireSelfLink()
                          .RequireRoutedLink("getall", "GetAllCategories")
                          .RequireRoutedLink("getById", "GetCategoryById", x => new { id = x.Id })
                          .RequireRoutedLink("update", "UpdateCategory", x => new { id = x.Id })
                          .RequireRoutedLink("add", "AddCategory")
                          .RequireRoutedLink("delete", "DeleteModelRoute", x => new { id = x.Id });
                });
            });

            services.AddApplicationInsightsTelemetry();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            var apiName = Configuration.GetValue<string>("ApiName");
            var apiSecret = Configuration.GetValue<string>("ApiSecret");
            var logger = loggerFactory.CreateLogger<Startup>();

            app.UseExceptionHandler(appError =>
            {
                appError.Run(context =>
                {
                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                    logger.LogError("The error has occured. Status: {status}. Details: {ex}", context.Response.StatusCode, contextFeature.Error);
                    return System.Threading.Tasks.Task.CompletedTask;
                });
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors("default");

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseOpenApi();

            app.UseSwaggerUi3(options =>
            {
                options.OAuth2Client = new OAuth2ClientSettings
                {
                    ClientId = apiName,
                    ClientSecret = apiSecret,
                    AppName = apiName,
                    UsePkceWithAuthorizationCodeGrant = false,
                    Scopes = { "catalog", "profile", "role" }
                };
            });
        }
    }
}

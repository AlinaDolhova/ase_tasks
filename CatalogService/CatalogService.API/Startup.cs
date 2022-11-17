using CatalogService.DAL;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore.SqlServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CatalogService.BLL.Interfaces;
using CatalogService.BLL.Services;
using CatalogService.DAL.Interfaces;
using RiskFirst.Hateoas;
using CatalogService.Model;
using Azure.Messaging.ServiceBus;
using IdentityServer4.Models;
using NSwag;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Authentication;

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
            services.AddControllers(options =>
            {
                options.Filters.Add(new AuthorizeFilter());
            });

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = "Cookies";
                options.DefaultChallengeScheme = "oidc";
            })
            .AddCookie("Cookies")             
            .AddOpenIdConnect("oidc", options =>
            {
                options.Authority = "https://localhost:44325";
                options.ClientId = "catalog";
                options.ClientSecret = "secret".Sha256();
                options.ResponseType = "code";
                options.SaveTokens = true;
                options.UsePkce = true;
                options.ClaimActions.MapJsonKey("role", "role", "role");
                options.TokenValidationParameters.RoleClaimType = "role";

                options.GetClaimsFromUserInfoEndpoint = true;
            });
            

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
            services.AddAutoMapper(typeof(Startup));

            services.AddSwaggerDocument(x=> 
            {
                x.AddSecurity("Bearer", new NSwag.OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = OpenApiSecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = OpenApiSecurityApiKeyLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme."
                });                
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
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseOpenApi();
            app.UseSwaggerUi3();
        }
    }
}

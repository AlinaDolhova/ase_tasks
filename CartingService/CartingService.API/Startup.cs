using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using CartingService.BLL;
using CartingService.BLL.Interfaces;
using CartingService.DAL;
using CartingService.DAL.Interfaces;
using LiteDB;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace CartingService.API
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
            var connectionString = Configuration.GetSection("LiteDB").GetValue<string>("DatabaseLocation");

            services.AddSingleton<ILiteDatabase, LiteDatabase>(x => new LiteDatabase(connectionString));

            var messageBusConnectionString = Configuration.GetConnectionString("ServiceBus");
            var clientOptions = new ServiceBusClientOptions()
            {
                TransportType = ServiceBusTransportType.AmqpWebSockets
            };

            services.AddSingleton(typeof(ServiceBusClient), new ServiceBusClient(messageBusConnectionString, clientOptions));
            services.AddSingleton(typeof(ServiceBusAdministrationClient), new ServiceBusAdministrationClient(messageBusConnectionString));

            services.AddScoped<ICartService, CartService>();
            services.AddScoped<ICartRepository, CartReporitory>();
            services.AddScoped<IConfigurationService, ConfigurationService>();
            services.AddScoped<IServiceBusConsumerService, ServiceBusConsumerService>();

            services.AddApiVersioning(options =>
            {
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.ReportApiVersions = true;
                options.ApiVersionReader = ApiVersionReader.Combine(
                        new HeaderApiVersionReader("X-Api-Version"),
                        new QueryStringApiVersionReader("version"));
            });
            services.AddAutoMapper(typeof(Startup));
            services.AddControllers();
            services.AddSwaggerGen(c=> {
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);            
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

            app.UseStaticFiles();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                c.RoutePrefix = string.Empty;
            });

            app.UseRouting();

            app.UseAuthorization();
           

            var bus = app.ApplicationServices.GetService<IServiceBusConsumerService>();
            bus.PrepareFiltersAndHandleMessages().GetAwaiter().GetResult();

        }
    }
}

using Azure.Messaging.ServiceBus;
using CatalogService.BLL.Interfaces;
using CatalogService.BLL.Services;
using CatalogService.Common.MapperProfiles;
using CatalogService.DAL;
using CatalogService.DAL.Interfaces;
using CatalogService.GraphQL.MapperProfiles;
using CatalogService.GraphQL.Models;
using CatalogService.Model;
using GraphQL.AspNet.Configuration.Mvc;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;

namespace CatalogService.GraphQL
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

            services.AddGraphQL(o =>
            {
                o.AddGraphType<ItemViewModel>();
                o.AddGraphType<CategoryViewModel>();
                o.AddGraphType<IdNamePair>();
                o.AddGraphType<Category>();
                o.AddGraphType<Item>();
            });

            services.AddAutoMapper(typeof(GraphQlMappingProfile), typeof(CatalogProfile));

            services.AddControllers()
              .AddNewtonsoftJson(
                options =>
                {
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseGraphQL();
        }
    }
}

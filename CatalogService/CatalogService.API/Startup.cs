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
            var connectionString = Configuration.GetConnectionString("CatalogDbContext");
            services.AddDbContext<CatalogDbContext>(options => options.UseSqlServer(connectionString));

            services.AddScoped<IItemService, ItemService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddAutoMapper(typeof(Startup));

            services.AddControllers();
            services.AddSwaggerDocument();

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
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapControllers();
            //});

            app.UseOpenApi();
            app.UseSwaggerUi3();

            app.UseMvc();
        }
    }
}

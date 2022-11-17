using CatalogService.Auth.Models;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace CatalogService.Auth.Data
{
    public static class DatabaseInitializer
    {
        public async static void PopulateIdentityServer(IApplicationBuilder app, ApplicationDbContext dbContext)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();

                var context = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();

                context.Database.Migrate();

                if (!context.Clients.Any())
                {
                    foreach (var client in Config.Clients)
                    {
                        context.Clients.Add(client.ToEntity());
                    }

                    context.SaveChanges();
                }

                if (!context.IdentityResources.Any())
                {
                    foreach (var resource in Config.IdentityResources)
                    {
                        context.IdentityResources.Add(resource.ToEntity());
                    }

                    context.SaveChanges();
                }

                if (!context.ApiResources.Any())
                {
                    foreach (var resource in Config.ApiResources)
                    {
                        context.ApiResources.Add(resource.ToEntity());
                    }

                    context.SaveChanges();
                }


                var roleStore = new RoleStore<IdentityRole>(dbContext);
                if (!roleStore.Roles.Any(x => x.Name == "MANAGER"))
                {
                    var managerRole = await roleStore.CreateAsync(new IdentityRole("MANAGER"));
                }

                if (!roleStore.Roles.Any(x => x.Name == Role.Buyer.ToString()))
                {
                    var buyerRole = await roleStore.CreateAsync(new IdentityRole(Role.Buyer.ToString()));
                }
            }
        }
    }
}

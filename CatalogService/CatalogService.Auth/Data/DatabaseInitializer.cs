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


                foreach (var scope in Config.ApiScopes)
                {
                    var existingScope = context.ApiScopes.FirstOrDefault(x => x.Name == scope.Name);
                    if (existingScope == null)
                    {
                        context.ApiScopes.Add(scope.ToEntity());
                    }
                    else
                    {                        
                        context.ApiScopes.Remove(existingScope);
                        context.ApiScopes.Add(scope.ToEntity());
                    }
                }

                context.SaveChanges();


                foreach (var client in Config.Clients)
                {
                    var existingClient = context.Clients.FirstOrDefault(x => x.ClientId == client.ClientId);
                    if (existingClient == null)
                    {
                        context.Clients.Add(client.ToEntity());
                    }
                    else
                    {
                        context.Clients.Remove(existingClient);
                        context.Clients.Add(client.ToEntity());
                    }
                }
                context.SaveChanges();



                foreach (var resource in Config.IdentityResources)
                {
                    var existingResource = context.IdentityResources.FirstOrDefault(x => x.Name == resource.Name);
                    if (existingResource == null)
                    {
                        context.IdentityResources.Add(resource.ToEntity());
                    }
                    else
                    {
                        context.IdentityResources.Remove(existingResource);
                        context.IdentityResources.Add(resource.ToEntity());
                    }
                }
                context.SaveChanges();


                foreach (var resource in Config.ApiResources)
                {
                    var existingResource = context.ApiResources.FirstOrDefault(x => x.Name == resource.Name);

                    if (existingResource == null)
                    {
                        context.ApiResources.Add(resource.ToEntity());
                    }
                    else
                    {
                        context.ApiResources.Remove(existingResource);
                        context.ApiResources.Add(resource.ToEntity());
                    }
                }

                context.SaveChanges();



                var roleStore = new RoleStore<IdentityRole>(dbContext);
                if (!roleStore.Roles.Any(x => x.Name == "MANAGER"))
                {
                    var managerRole = await roleStore.CreateAsync(new IdentityRole("MANAGER"));
                }

                if (!roleStore.Roles.Any(x => x.Name == Role.Buyer.ToString()))
                {
                    var buyerRole = await roleStore.CreateAsync(new IdentityRole(Role.Buyer.ToString()));
                }

                dbContext.SaveChanges();
            }
        }
    }
}

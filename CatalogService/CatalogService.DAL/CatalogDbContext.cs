using CatalogService.DAL.Interfaces;
using CatalogService.DAL.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace CatalogService.DAL
{
    public class CatalogDbContext : DbContext, IDbContext
    {
        public DbSet<Category> Categories { get; set; }
        public DbSet<Item> Items { get; set; }

        public async Task SaveChangesAsync()
        {
            await this.SaveChangesAsync();
        }
    }
}

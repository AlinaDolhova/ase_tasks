using System.Threading.Tasks;
using CatalogService.DAL.Interfaces;
using CatalogService.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.DAL
{
    public class CatalogDbContext : DbContext, IDbContext
    {
        public CatalogDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Item> Items { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<Category>()
                .HasMany(e => e.ChildCategories)
                .WithOne(x => x.ParentCategory)
                .HasForeignKey(x => x.ParentCategoryId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder
              .Entity<Category>()
              .HasMany(x => x.Items)
              .WithOne(x => x.Category)
              .HasForeignKey(x => x.CategoryId)
              .OnDelete(DeleteBehavior.ClientCascade);
        }
    }
}

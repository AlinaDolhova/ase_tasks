using CatalogService.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatalogService.DAL
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class, IIdentifiable, IDeletable
    {
        private readonly DbSet<T> entities;
        private readonly CatalogDbContext dbContext;

        public GenericRepository(CatalogDbContext dbContext)
        {
            this.entities = dbContext.Set<T>();
            this.dbContext = dbContext;

        }

        public async Task AddAsync(T entity)
        {
            this.entities.Add(entity);
            await this.dbContext.SaveChangesAsync();

        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await GetByIdAsync(id);
            entity.IsDeleted = true;

            await this.UpdateAsync(entity);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await this.entities.Where(x => !x.IsDeleted).ToListAsync();
        }

        public async Task<T> GetByIdAsync(Guid id)
        {
            return await this.entities.Where(x => !x.IsDeleted && x.Id == id).FirstOrDefaultAsync();
        }

        public async Task UpdateAsync(T entity)
        {
            this.entities.Update(entity);
            await this.dbContext.SaveChangesAsync();
        }
    }
}

using CatalogService.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

        public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> filter = null,
        Func<IQueryable<T>, IOrderedQueryable<T>> order = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null,
        int skip = 0, int take = int.MaxValue)
        {
            IQueryable<T> query = this.entities.Where(x => !x.IsDeleted);

            query = skip == 0 ? query.Take(take) : query.Skip(skip).Take(take);
            query = filter is null ? query : query.Where(filter);
            query = order is null ? query : order(query);
            query = include is null ? query : include(query);

            return await query.ToListAsync();
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

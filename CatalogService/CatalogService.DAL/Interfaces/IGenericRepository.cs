using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CatalogService.DAL.Interfaces
{
    public interface IGenericRepository<T> where T : IIdentifiable, IDeletable
    {
        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> filter = null,
        Func<IQueryable<T>, IOrderedQueryable<T>> order = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null,
        int skip = 0, int take = int.MaxValue);

        Task<T> GetByIdAsync(Guid id);

        Task AddAsync(T entity);

        Task UpdateAsync(T entity);

        Task DeleteAsync(Guid id);
    }
}

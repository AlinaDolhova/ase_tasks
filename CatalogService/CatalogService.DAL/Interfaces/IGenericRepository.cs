using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CatalogService.DAL.Interfaces
{
    public interface IGenericRepository<T> where T: IIdentifiable
    {
        Task<IEnumerable<T>> GetAllAsync();

        Task<T> GetByIdAsync(Guid id);

        Task AddAsync(T entity);

        Task UpdateAsync(T entity);

        Task DeleteAsync(Guid id);
    }
}

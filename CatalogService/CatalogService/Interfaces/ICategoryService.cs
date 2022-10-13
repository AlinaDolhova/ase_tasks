using CategoryService.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CatalogService.BLL.Interfaces
{
    public interface ICategoryService
    {
        //Category: get/list/add/update/delete

        Task<Category> GetAsync(Guid id);

        Task<IEnumerable<Category>> GetAsync();

        Task AddAsync(Category item);

        Task UpdateAsync(Guid id, Category item);

        Task DeleteAsync(Guid id);
    }
}

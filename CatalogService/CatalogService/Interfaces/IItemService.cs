using CatalogService.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CatalogService.BLL.Interfaces
{
    public interface IItemService
    {
        //Item: get/list/add/update/delete.

        Task<Item> GetAsync(Guid id);

        Task<IEnumerable<Item>> GetAsync();

        Task AddAsync(Item item);

        Task UpdateAsync(Guid id, Item item);

        Task DeleteAsync(Guid id);
    }
}

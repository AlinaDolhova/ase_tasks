using AutoMapper;
using CatalogService.BLL.Interfaces;
using CatalogService.DAL.Interfaces;
using CatalogService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CatalogService.BLL.Services
{
    public class ItemService : IItemService
    {
        private readonly IGenericRepository<DAL.Models.Item> itemRepository;
        private readonly IMapper mapper;

        public ItemService(IGenericRepository<DAL.Models.Item> categoryRepo, IMapper mapper)
        {
            this.itemRepository = categoryRepo;
            this.mapper = mapper;
        }

        public async Task AddAsync(Item item)
        {
            ValidateItem(item);

            var itemToInsert = mapper.Map<DAL.Models.Item>(item);
            await this.itemRepository.AddAsync(itemToInsert);
        }

        public async Task DeleteAsync(Guid id)
        {
            var itemFromDb = await GetAsync(id);
            if (null == itemFromDb)
            {
                throw new KeyNotFoundException("Item with such id does not exist");
            }

            await this.itemRepository.DeleteAsync(id);
        }

        public async Task<Item> GetAsync(Guid id)
        {
            var itemFromDb = await itemRepository.GetByIdAsync(id);

            if (itemFromDb != null)
            {
                return mapper.Map<Item>(itemFromDb);
            }

            return null;
        }

        public async Task<IEnumerable<Item>> GetAsync() => (await itemRepository.GetAllAsync()).Select(x => mapper.Map<Item>(x));

        public async Task UpdateAsync(Guid id, Item item)
        {
            var itemFromDb = await itemRepository.GetByIdAsync(id);
            if (null == itemFromDb)
            {
                throw new KeyNotFoundException("Item with such id does not exist");
            }

            ValidateItem(item);

            mapper.Map(item, itemFromDb);

            await itemRepository.UpdateAsync(itemFromDb);
        }

        private void ValidateItem(Item item)
        {
            if (null == item)
            {
                throw new ArgumentNullException("Item can't be null");
            }

            if (string.IsNullOrEmpty(item.Name))
            {
                throw new ArgumentException("Item's name can't be empty");
            }

            if (Guid.Empty == item.CategoryId)
            {
                throw new ArgumentException("Item's category id can't be empty");
            }

            if (0 >= item.Amount)
            {
                throw new ArgumentException("Item's amount should be a positive value");
            }

            if (0 >= item.Money)
            {
                throw new ArgumentException("Item's money should be a positive value");
            }
        }
    }
}

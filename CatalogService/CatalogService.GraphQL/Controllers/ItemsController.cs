using CatalogService.BLL.Interfaces;
using GraphQL.AspNet.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using CatalogService.Model;
using GraphQL.AspNet.Attributes;
using CatalogService.GraphQL.Models;
using AutoMapper;
using System.Linq;

namespace CatalogService.GraphQL.Controllers
{
    [ApiController]
    public class ItemsController : GraphController
    {
        private readonly IItemService itemService;
        private readonly ILogger<ItemsController> logger;
        private readonly IMapper mapper;

        public ItemsController(IItemService itemService, ILogger<ItemsController> logger, IMapper mapper)
        {
            this.itemService = itemService;
            this.logger = logger;
            this.mapper = mapper;
        }

        [QueryRoot("items")]
        public async Task<IEnumerable<ItemViewModel>> GetAsync(Guid categoryId, int page = 0, int perPage = 10)
        {
            logger.LogInformation("Getting all items for category {id}", categoryId);
            var items = await itemService.GetAsync(categoryId, page, perPage);

            return items.Select(x => mapper.Map<ItemViewModel>(x));
        }


        [QueryRoot("item")]
        public async Task<ItemViewModel> GetItemByIdAsync(Guid id)
        {
            logger.LogInformation("Getting item {id}", id);
            return mapper.Map<ItemViewModel>(await itemService.GetAsync(id));
        }


        [MutationRoot("addItem")]
        public async Task<ItemViewModel> AddAsync(ItemInput item)
        {
            try
            {
                if (item.Id == Guid.Empty)
                {
                    item.Id = Guid.NewGuid();
                }

                logger.LogInformation("Adding new item {itemId} for category {categoryId}", item.Id, item.CategoryId);

                await itemService.AddAsync(mapper.Map<Item>(item));

                return mapper.Map<ItemViewModel>(await itemService.GetAsync(item.Id));
            }
            catch (Exception ex) when (ex is ArgumentException ||
                               ex is ArgumentNullException)
            {

                return null;
            }
        }

        [MutationRoot("updateItem")]
        public async Task<ItemViewModel> UpdateAsync(Guid id, ItemInput item)
        {
            logger.LogInformation("Updating item {itemId} for category {categoryId}", item.Id, item.CategoryId);

            try
            {
                await itemService.UpdateAsync(id, mapper.Map<Item>(item));

                return mapper.Map<ItemViewModel>(await itemService.GetAsync(id));
            }
            catch (Exception ex) when (ex is ArgumentException ||
                               ex is ArgumentNullException)
            {

                return null;
            }
        }

        [MutationRoot("deleteItem")]
        public async Task<string> DeleteAsync(Guid id)
        {
            logger.LogInformation("Deleting item {itemId}", id);

            try
            {
                await itemService.DeleteAsync(id);
                return "Item was deleted";
            }
            catch
            {
                return "Item was not deleted";
            }
        }

    }
}

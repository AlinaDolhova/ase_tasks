using CatalogService.BLL.Interfaces;
using CatalogService.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CatalogService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemsController : ControllerBase
    {
        private readonly IItemService itemService;

        public ItemsController(IItemService itemService)
        {
            this.itemService = itemService;
        }

        [HttpGet]
        public async Task<IEnumerable<Item>> GetAsync(Guid categoryId, int page, int perPage)
        {
            return await itemService.GetAsync();
        }

        [HttpPost]
        public async Task AddAsync(Item item)
        {
            await itemService.AddAsync(item);
        }

        [HttpPatch]
        public async Task UpdateAsync(Guid id, Item item)
        {
            await itemService.UpdateAsync(id, item);
        }

        [HttpDelete]
        public async Task DeleteAsync(Guid id)
        {
            await itemService.DeleteAsync(id);
        }
    }
}

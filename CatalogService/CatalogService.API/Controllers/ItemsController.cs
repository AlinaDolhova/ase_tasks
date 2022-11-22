using CatalogService.BLL.Interfaces;
using CatalogService.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
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

        [AllowAnonymous]
        [HttpGet("category/{categoryId}", Name = "GetAllItemsByCategoryId")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Item>>> GetAsync(Guid categoryId, int page = 0, int perPage = 10)
        {
            return Ok(await itemService.GetAsync(categoryId, page, perPage));
        }

        /// <summary>
        /// Extend catalog service with new endpoint which returns a list of CatalogItem 
        /// properties for the specific CatalogItem. Dictionary of key/value pairs 
        /// (for example: brand = Samsung, model = s10). Note! You can hardcode return data to save time.
        /// </summary>
        /// <param name="itemId"> The id of the item</param>
        /// <returns></returns>
        [HttpGet("{id}/properties")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Dictionary<string, string>>> GetPropertiesOfItemAsync(Guid id)
        {
            return Ok(new Dictionary<string, string>() { { "brand", "Samsung" }, { "model", "s10" } });
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Item>> GetItemByIdAsync(Guid id)
        {
            return Ok(await itemService.GetAsync(id));
        }


        [Authorize(Roles = "Manager,Admin")]
        [HttpPost(Name = "AddItem")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> AddAsync(Item item)
        {
            try
            {
                item.Id = Guid.NewGuid();
                await itemService.AddAsync(item);

                return CreatedAtAction(nameof(GetAsync), new { id = item.Id }, item);
            }
            catch (Exception ex) when (ex is ArgumentException ||
                               ex is ArgumentNullException)
            {

                return BadRequest(ex);
            }
        }

        [Authorize(Roles = "Manager,Admin")]
        [HttpPatch("{id}", Name = "UpdateItem")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> UpdateAsync(Guid id, Item item)
        {
            try
            {
                await itemService.UpdateAsync(id, item);

                return Ok();
            }
            catch (Exception ex) when (ex is ArgumentException ||
                               ex is ArgumentNullException)
            {

                return BadRequest(ex);
            }
        }

        [Authorize(Roles = "Manager,Admin")]
        [HttpDelete("{id}", Name = "DeleteItem")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteAsync(Guid id)
        {
            try
            {
                await itemService.DeleteAsync(id);
                return Ok();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex);
            }
        }
    }
}

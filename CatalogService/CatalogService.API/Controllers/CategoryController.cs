using CatalogService.BLL.Interfaces;
using CatalogService.Model;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Net.Mime;
using System.Threading.Tasks;

namespace CatalogService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService categoryService;
        private readonly ILogger<CategoryController> logger;

        public CategoryController(ICategoryService categoryService, ILogger<CategoryController> logger)
        {
            this.categoryService = categoryService;
            this.logger = logger;
           
        }

        [AllowAnonymous]
        [HttpGet(Name = "GetAllCategories")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Category>>> GetAsync()
        {
            logger.LogInformation("Getting all categories");
            return Ok(await categoryService.GetAsync());
        }

        [AllowAnonymous]
        [HttpGet("{id}", Name = "GetCategoryById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<Category>>> GetAsync(Guid id)
        {
            logger.LogInformation("Getting category with id {id}", id);
            var result = await categoryService.GetAsync(id);
            if (result == null)
            {
                logger.LogWarning("Getting category with id {id} - not found", id);
                return NotFound();
            }

            return Ok(result);
        }

        [Authorize(Roles = "Manager,Admin")]
        [HttpPost(Name = "AddCategory")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> AddAsync(Category category)
        {
            try
            {
                if (category.Id == Guid.Empty)
                {
                    category.Id = Guid.NewGuid();
                }
                logger.LogInformation("Adding category with id {id}", category.Id);

                await categoryService.AddAsync(category);

                return CreatedAtAction(nameof(GetAsync), new { id = category.Id });
            }
            catch (Exception ex) when (ex is ArgumentException ||
                               ex is ArgumentNullException)
            {

                return BadRequest(ex);
            }
        }

        [Authorize(Roles = "Manager,Admin")]
        [HttpPatch("{id}", Name = "UpdateCategory")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> UpdateAsync(Guid id, Category category)
        {
            logger.LogInformation("Updating category with id {id}", id);
            try
            {
                await categoryService.UpdateAsync(id, category);

                return Ok();
            }
            catch (Exception ex) when (ex is ArgumentException ||
                               ex is ArgumentNullException)
            {

                return BadRequest(ex);
            }
        }

        [Authorize(Roles = "Manager,Admin")]
        [HttpDelete("{id}", Name = "DeleteCategory")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            logger.LogInformation("Deleting category with id {id}", id);

            try
            {
                await categoryService.DeleteAsync(id);
                return Ok();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex);
            }
        }

    }
}

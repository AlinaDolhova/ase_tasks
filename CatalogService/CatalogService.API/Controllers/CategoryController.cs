﻿using CatalogService.BLL.Interfaces;
using CatalogService.Model;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
    

        public CategoryController(ICategoryService categoryService)
        {
            this.categoryService = categoryService;
           
        }

        [AllowAnonymous]
        [HttpGet(Name = "GetAllCategories")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Category>>> GetAsync()
        {
            return Ok(await categoryService.GetAsync());
        }

        [AllowAnonymous]
        [HttpGet("{id}", Name = "GetCategoryById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<Category>>> GetAsync(Guid id)
        {
            var result = await categoryService.GetAsync(id);
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [Authorize(Roles = "Manager")]
        [HttpPost(Name = "AddCategory")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> AddAsync(Category category)
        {
            try
            {
                category.Id = Guid.NewGuid();
                await categoryService.AddAsync(category);

                return CreatedAtAction(nameof(GetAsync), new { id = category.Id });
            }
            catch (Exception ex) when (ex is ArgumentException ||
                               ex is ArgumentNullException)
            {

                return BadRequest(ex);
            }
        }

        [Authorize(Roles = "Manager")]
        [HttpPatch("{id}", Name = "UpdateCategory")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> UpdateAsync(Guid id, Category category)
        {
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

        [Authorize(Roles = "Manager")]
        [HttpDelete("{id}", Name = "DeleteCategory")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
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

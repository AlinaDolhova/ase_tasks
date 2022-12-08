using CatalogService.BLL.Interfaces;
using GraphQL.AspNet.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading.Tasks;
using System;
using GraphQL.AspNet.Attributes;
using CatalogService.Model;
using CatalogService.GraphQL.Models;
using AutoMapper;
using System.Linq;

namespace CatalogService.GraphQL.Controllers
{   
    [ApiController]
    public class CategoryController : GraphController
    {
        private readonly ICategoryService categoryService;
        private readonly ILogger<CategoryController> logger;
        private readonly IMapper mapper;

        public CategoryController(ICategoryService categoryService, ILogger<CategoryController> logger, IMapper mapper)
        {
            this.categoryService = categoryService;
            this.logger = logger;
            this.mapper = mapper;

        }

        [QueryRoot("categories")]
        public async Task<IEnumerable<CategoryViewModel>> GetAsync()
        {
            logger.LogInformation("Getting all categories");
            return (await categoryService.GetAsync()).Select(x => mapper.Map<CategoryViewModel>(x));
        }

        [QueryRoot("category")]
        public async Task<CategoryViewModel> GetAsync(Guid id)
        {
            logger.LogInformation("Getting category with id {id}", id);
            var result = await categoryService.GetAsync(id);

            return mapper.Map<CategoryViewModel>(result);
        }

        [MutationRoot("addCategory")]
        public async Task<Category> AddAsync(CategoryInput category)
        {
            try
            {
                if (category.Id == Guid.Empty)
                {
                    category.Id = Guid.NewGuid();
                }

                logger.LogInformation("Adding category with id {id}", category.Id);

                await categoryService.AddAsync(mapper.Map<Category>(category));

                return await categoryService.GetAsync(category.Id);
            }
            catch (Exception ex) when (ex is ArgumentException ||
                               ex is ArgumentNullException)
            {

                logger.LogError("The error  has occured while adding new category", ex);
                return null;
            }
        }

        [MutationRoot("updateCategory")]
        public async Task<Category> UpdateAsync(Guid id, CategoryInput category)
        {
            logger.LogInformation("Updating category with id {id}", id);
            try
            {
                await categoryService.UpdateAsync(id, mapper.Map<Category>(category));

                return await categoryService.GetAsync(id);
            }
            catch (Exception ex) when (ex is ArgumentException ||
                               ex is ArgumentNullException)
            {

                logger.LogError("The error  has occured while updating the category", ex);
                return null;
            }
        }

        [MutationRoot("deleteCategory")]
        public async Task<string> DeleteAsync(Guid id)
        {
            logger.LogInformation("Deleting category with id {id}", id);

            try
            {
                await categoryService.DeleteAsync(id);
                return "Category was deleted";
            }
            catch
            {
                return "Category was not deleted";
            }
        }

    }
}

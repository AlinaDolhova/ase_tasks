using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CatalogService.BLL.Interfaces;
using CatalogService.DAL.Interfaces;
using CatalogService.Model;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.BLL.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IGenericRepository<DAL.Models.Category> categoryRepository;
        private readonly IMapper mapper;

        public CategoryService(IGenericRepository<DAL.Models.Category> categoryRepo, IMapper mapper)
        {
            this.categoryRepository = categoryRepo;
            this.mapper = mapper;
        }

        public async Task AddAsync(Category item)
        {
            Validate(item);

            var categoryToInsert = mapper.Map<DAL.Models.Category>(item);
            await this.categoryRepository.AddAsync(categoryToInsert);
        }

        public async Task DeleteAsync(Guid id)
        {
            var category = await GetAsync(id);
            if (null == category)
            {
                throw new KeyNotFoundException("Category with such id does not exist");
            }

            await this.categoryRepository.DeleteAsync(id);
        }

        public async Task<Category> GetAsync(Guid id, bool includeItems = true)
        {
            var categoriesList = await categoryRepository.GetAllAsync(x => x.Id == id, include: x => x.Include(t => t.Items));
            var category = categoriesList.FirstOrDefault();

            if (category != null)
            {
                return mapper.Map<Category>(category);
            }

            return null;
        }

        public async Task<IEnumerable<Category>> GetAsync()
        {
            return (await categoryRepository.GetAllAsync(include: x => x.Include(t => t.Items))).Select(x => mapper.Map<Category>(x));
        }

        public async Task UpdateAsync(Guid id, Category item)
        {
            var category = await categoryRepository.GetByIdAsync(id);
            if (null == category)
            {
                throw new KeyNotFoundException("Category with such id does not exist");
            }

            Validate(item);

            mapper.Map(item, category);

            await categoryRepository.UpdateAsync(category);
        }

        private void Validate(Category category)
        {
            if (null == category)
            {
                throw new ArgumentNullException("category", "Category can't be null");
            }

            if (string.IsNullOrEmpty(category.Name))
            {
                throw new ArgumentException("Category name can't be empty");
            }
        }
    }
}

using AutoMapper;
using CatalogService.BLL.Interfaces;
using CatalogService.DAL.Interfaces;
using CategoryService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            if (item == null)
            {
                throw new ArgumentNullException("Category can't be null");
            }

            if (string.IsNullOrEmpty(item.Name))
            {
                throw new ArgumentException("Category name can't be empty");
            }

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

        public async Task<Category> GetAsync(Guid id)
        {
            var category = await categoryRepository.GetByIdAsync(id);

            if (category != null)
            {
                return mapper.Map<Category>(category);
            }

            return null;
        }

        public async Task<IEnumerable<Category>> GetAsync() => (await categoryRepository.GetAllAsync()).Select(x => mapper.Map<Category>(x));

        public async Task UpdateAsync(Guid id, Category item)
        {
            var category = await categoryRepository.GetByIdAsync(id);
            if (null == category)
            {
                throw new KeyNotFoundException("Category with such id does not exist");
            }

            if (item == null)
            {
                throw new ArgumentNullException("Category can't be null");
            }

            if (string.IsNullOrEmpty(item.Name))
            {
                throw new ArgumentException("Category name can't be empty");
            }

            mapper.Map(item, category);

            await categoryRepository.UpdateAsync(category);
        }
    }
}

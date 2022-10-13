using AutoMapper;
using CatalogService.BLL.Services;
using CatalogService.DAL.Interfaces;
using CatalogService.DAL.Models;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CatalogService.Tests
{
    public class CategoryServiceTests
    {
        private Mock<IGenericRepository<Category>> repoMock;
        private BLL.Services.CategoryService categoryService;
        private Mock<IMapper> mapperMock;

        [SetUp]
        public void Setup()
        {
            repoMock = new Mock<IGenericRepository<Category>>();
            mapperMock = new Mock<IMapper>();

            mapperMock.Setup(x => x.Map<Model.Category>(It.IsAny<Category>())).Returns((Category x) => new Model.Category { Id = x.Id, Name = x.Name, ImageUrl = x.ImageUrl });
            mapperMock.Setup(x => x.Map<Category>(It.IsAny<Model.Category>())).Returns((Model.Category x) => new Category { Id = x.Id, Name = x.Name, ImageUrl = x.ImageUrl });

            mapperMock.Setup(x => x.Map(It.IsAny<Model.Category>(), It.IsAny<Category>())).Callback((Model.Category c1, Category c2) => c2.Name = c1.Name);
        }

        [Test]
        public async Task AddAsync_AddsCategory()
        {
            var category = new Model.Category { Name = "test" };
            categoryService = new CategoryService(repoMock.Object, mapperMock.Object);
            await categoryService.AddAsync(category);

            repoMock.Verify(x => x.AddAsync(It.Is<Category>(x => x.Name == category.Name)));
        }

        [Test]
        public void AddAsync_ValidatesName()
        {
            var category = new Model.Category();
            categoryService = new CategoryService(repoMock.Object, mapperMock.Object);
            Assert.ThrowsAsync<ArgumentException>(() => categoryService.AddAsync(category));

        }

        [Test]
        public void AddAsync_ValidatesNullCategory()
        {
            categoryService = new CategoryService(repoMock.Object, mapperMock.Object);
            Assert.ThrowsAsync<ArgumentNullException>(() => categoryService.AddAsync(null));
        }

        [Test]
        public void UpdateAsync_ValidatesName()
        {
            var category = new Model.Category();
            var testId = Guid.NewGuid();

            repoMock.Setup(x => x.GetByIdAsync(testId)).ReturnsAsync(new Category { Id = testId });
            categoryService = new CategoryService(repoMock.Object, mapperMock.Object);

            Assert.ThrowsAsync<ArgumentException>(() => categoryService.UpdateAsync(testId, category));
        }

        [Test]
        public async Task UpdateAsync_UpdatesCategory()
        {
            var category = new Model.Category() { Name = "new" };
            var testId = Guid.NewGuid();

            repoMock.Setup(x => x.GetByIdAsync(testId)).ReturnsAsync(new Category { Id = testId, Name = "old" });
            categoryService = new CategoryService(repoMock.Object, mapperMock.Object);

            await categoryService.UpdateAsync(testId, category);

            repoMock.Verify(x => x.UpdateAsync(It.Is<Category>(x => x.Name == category.Name)));
        }

        [Test]
        public async Task DeleteAsync_DeletesCategory()
        {
            var testId = Guid.NewGuid();

            repoMock.Setup(x => x.GetByIdAsync(testId)).ReturnsAsync(new Category { Id = testId, Name = "old" });
            categoryService = new CategoryService(repoMock.Object, mapperMock.Object);

            await categoryService.DeleteAsync(testId);

            repoMock.Verify(x => x.DeleteAsync(testId));
        }


        [Test]
        public void DeleteAsync_WontDeleteNotExistingCategory()
        {            
            var testId = Guid.NewGuid();
            
            categoryService = new CategoryService(repoMock.Object, mapperMock.Object);

            Assert.ThrowsAsync<KeyNotFoundException>(() => categoryService.DeleteAsync(testId));
        }
    }
}
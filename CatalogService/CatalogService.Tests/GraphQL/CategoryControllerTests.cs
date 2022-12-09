using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using CatalogService.BLL.Interfaces;
using CatalogService.GraphQL.Controllers;
using CatalogService.GraphQL.Models;
using CatalogService.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace CatalogService.Tests.GraphQL
{
    internal class CategoryControllerTests
    {
        private CategoryController categoryController;
        private Mock<ICategoryService> categoryServiceMock;
        private Mock<ILogger<CategoryController>> loggerMock;
        private Mock<IMapper> mapper;

        [SetUp]
        public void Setup()
        {
            this.loggerMock = new Mock<ILogger<CategoryController>>();
            this.categoryServiceMock = new Mock<ICategoryService>();
            this.mapper = new Mock<IMapper>();
        }

        [Test]
        public async Task CategoryController_GetAsync_OK()
        {
            categoryServiceMock.Setup(x => x.GetAsync()).ReturnsAsync(new List<Category> { new Category() });
            mapper.Setup(x => x.Map<CategoryViewModel>(It.IsAny<Category>())).Returns(new CategoryViewModel());

            categoryController = new CategoryController(categoryServiceMock.Object, loggerMock.Object, mapper.Object);
            var result = await categoryController.GetAsync();

            Assert.That(result.Count(), Is.EqualTo(1));
        }

        [Test]
        public async Task CategoryController_GetAsync_WithId_OK()
        {
            var id = Guid.NewGuid();
            categoryServiceMock.Setup(x => x.GetAsync()).ReturnsAsync(new List<Category> { new Category { Id = id } });
            mapper.Setup(x => x.Map<CategoryViewModel>(It.IsAny<Category>())).Returns(new CategoryViewModel { Id = id });

            categoryController = new CategoryController(categoryServiceMock.Object, loggerMock.Object, mapper.Object);
            var result = await categoryController.GetAsync(id);

            Assert.That(result.Id, Is.EqualTo(id));
        }


        [Test]
        public async Task CategoryController_AddCategory_OK()
        {
            var name = "name";
            var id = Guid.NewGuid();
            mapper.Setup(x => x.Map<Category>(It.IsAny<CategoryInput>())).Returns<CategoryInput>(x => new Category { Id = x.Id, Name = x.Name });

            categoryController = new CategoryController(categoryServiceMock.Object, loggerMock.Object, mapper.Object);

            var result = await categoryController.AddAsync(new CategoryInput() { Id = id, Name = name });

            categoryServiceMock.Verify(x => x.AddAsync(It.Is<Category>(x => x.Id == id && x.Name == name)));
        }


        [Test]
        public async Task CategoryController_UpdateCategory_OK()
        {
            var name = "name";
            var id = Guid.NewGuid();
            mapper.Setup(x => x.Map<Category>(It.IsAny<CategoryInput>())).Returns<CategoryInput>(x => new Category { Id = x.Id, Name = x.Name });

            categoryController = new CategoryController(categoryServiceMock.Object, loggerMock.Object, mapper.Object);

            var result = await categoryController.UpdateAsync(id, new CategoryInput { Id = id, Name = name });

            categoryServiceMock.Verify(x => x.UpdateAsync(id, It.Is<Category>(x => x.Id == id && x.Name == name)));
        }

        [Test]
        public async Task CategoryController_DeleteCategory_NotFound()
        {
            var id = Guid.NewGuid();
            categoryServiceMock.Setup(x => x.DeleteAsync(id)).Throws(new KeyNotFoundException());
            categoryController = new CategoryController(categoryServiceMock.Object, loggerMock.Object, mapper.Object);

            var result = await categoryController.DeleteAsync(id);

            Assert.That(result, Is.EqualTo("Category was not deleted"));
        }

        [Test]
        public async Task CategoryController_DeleteCategory_OK()
        {
            var id = Guid.NewGuid();
            categoryController = new CategoryController(categoryServiceMock.Object, loggerMock.Object, mapper.Object);

            var result = await categoryController.DeleteAsync(id);

            Assert.That(result, Is.EqualTo("Category was deleted"));
        }
    }
}

using CatalogService.API.Controllers;
using CatalogService.BLL.Interfaces;
using CatalogService.BLL.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CatalogService.Tests.PL
{
    public class CategoryControllerTests
    {
        private CategoryController categoryController;
        private Mock<ICategoryService> categoryServiceMock;

        [SetUp]
        public void Setup()
        {
            this.categoryServiceMock = new Mock<ICategoryService>();
        }

        [Test]
        public async Task CategoryController_GetAsync_OK()
        {
            categoryController = new CategoryController(categoryServiceMock.Object);
            var okResult = (await categoryController.GetAsync());

            Assert.True(okResult.Result is OkObjectResult);
        }

        [Test]
        public async Task CategoryController_GetAsync_WithId_OK()
        {
            var id = Guid.NewGuid();
            categoryServiceMock.Setup(x => x.GetAsync(id)).ReturnsAsync(new Model.Category());
            categoryController = new CategoryController(categoryServiceMock.Object);

            var okResult = (await categoryController.GetAsync(id));

            Assert.True(okResult.Result is OkObjectResult);
        }

        [Test]
        public async Task CategoryController_GetAsync_WithId_NotFound()
        {
            var id = Guid.NewGuid();
            categoryController = new CategoryController(categoryServiceMock.Object);

            var result = (await categoryController.GetAsync(id));

            Assert.True(result.Result is NotFoundResult);
        }

        [Test]
        public async Task CategoryController_AddCategory_OK()
        {
            categoryController = new CategoryController(categoryServiceMock.Object);

            var result = await categoryController.AddAsync(new Model.Category());

            Assert.True(result is Microsoft.AspNetCore.Mvc.CreatedAtActionResult);
        }

        [Test]
        public async Task CategoryController_AddCategory_BadRequest()
        {
            categoryServiceMock.Setup(x => x.AddAsync(It.IsAny<Model.Category>())).Throws(new ArgumentException());
            categoryController = new CategoryController(categoryServiceMock.Object);

            var result = await categoryController.AddAsync(new Model.Category());

            Assert.True(result is BadRequestObjectResult);
        }

        [Test]
        public async Task CategoryController_UpdateCategory_OK()
        {
            var id = Guid.NewGuid();
            categoryController = new CategoryController(categoryServiceMock.Object);

            var result = await categoryController.UpdateAsync(id, new Model.Category());

            Assert.True(result is OkResult);
        }

        [Test]
        public async Task CategoryController_UpdateCategory_BadRequest()
        {
            var id = Guid.NewGuid();
            categoryServiceMock.Setup(x => x.UpdateAsync(id, It.IsAny<Model.Category>())).Throws(new ArgumentException());
            categoryController = new CategoryController(categoryServiceMock.Object);

            var result = await categoryController.UpdateAsync(id, new Model.Category());

            Assert.True(result is BadRequestObjectResult);
        }

        [Test]
        public async Task CategoryController_DeleteCategory_NotFound()
        {
            var id = Guid.NewGuid();
            categoryServiceMock.Setup(x => x.DeleteAsync(id)).Throws(new KeyNotFoundException());
            categoryController = new CategoryController(categoryServiceMock.Object);

            var result = await categoryController.DeleteAsync(id);

            Assert.True(result is NotFoundObjectResult);
        }

        [Test]
        public async Task CategoryController_DeleteCategory_OK()
        {
            var id = Guid.NewGuid();           
            categoryController = new CategoryController(categoryServiceMock.Object);

            var result = await categoryController.DeleteAsync(id);

            Assert.True(result is OkResult);
        }
    }
}

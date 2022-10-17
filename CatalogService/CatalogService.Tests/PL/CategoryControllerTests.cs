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
        public void CategoryController_GetAsync_WithId_OK()
        {

        }

        [Test]
        public void CategoryController_GetAsync_WithId_NotFound()
        {

        }

        [Test]
        public void CategoryController_AddCategory_OK()
        {

        }

        [Test]
        public void CategoryController_AddCategory_BadRequest()
        {

        }

        [Test]
        public void CategoryController_UpdateCategory_OK()
        {

        }

        [Test]
        public void CategoryController_UpdateCategory_BadRequest()
        {

        }

        [Test]
        public void CategoryController_DeleteCategory_NotFound()
        {

        }

        [Test]
        public void CategoryController_DeleteCategory_OK()
        {

        }
    }
}

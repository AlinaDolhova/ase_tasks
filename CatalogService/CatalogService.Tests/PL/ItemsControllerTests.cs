using CatalogService.API.Controllers;
using CatalogService.BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CatalogService.Tests.PL
{
    public class ItemsControllerTests
    {
        private ItemsController itemsController;
        private Mock<IItemService> itemServiceMock;
        private Mock<ILogger<ItemsController>> loggerMock;

        [SetUp]
        public void Setup()
        {
            this.loggerMock = new Mock<ILogger<ItemsController>>();
            this.itemServiceMock = new Mock<IItemService>();
        }
        

        [Test]
        public async Task ItemsController_AddItem_OK()
        {
            itemsController = new ItemsController(itemServiceMock.Object, loggerMock.Object);

            var result = await itemsController.AddAsync(new Model.Item());

            Assert.That(result is Microsoft.AspNetCore.Mvc.CreatedAtActionResult, Is.True);
        }

        [Test]
        public async Task ItemsController_AddItem_BadRequest()
        {
            itemServiceMock.Setup(x => x.AddAsync(It.IsAny<Model.Item>())).Throws(new ArgumentException());
            itemsController = new ItemsController(itemServiceMock.Object, loggerMock.Object);

            var result = await itemsController.AddAsync(new Model.Item());

            Assert.That(result is BadRequestObjectResult, Is.True);
        }

        [Test]
        public async Task ItemsController_UpdateItem_OK()
        {
            var id = Guid.NewGuid();
            itemsController = new ItemsController(itemServiceMock.Object, loggerMock.Object);

            var result = await itemsController.UpdateAsync(id, new Model.Item());

            Assert.That(result is OkResult, Is.True);
        }

        [Test]
        public async Task ItemsController_UpdateItem_BadRequest()
        {
            var id = Guid.NewGuid();
            itemServiceMock.Setup(x => x.UpdateAsync(id, It.IsAny<Model.Item>())).Throws(new ArgumentException());
            itemsController = new ItemsController(itemServiceMock.Object, loggerMock.Object);

            var result = await itemsController.UpdateAsync(id, new Model.Item());

            Assert.That(result is BadRequestObjectResult, Is.True);
        }

        [Test]
        public async Task ItemsController_DeleteItem_NotFound()
        {
            var id = Guid.NewGuid();
            itemServiceMock.Setup(x => x.DeleteAsync(id)).Throws(new KeyNotFoundException());
            itemsController = new ItemsController(itemServiceMock.Object, loggerMock.Object);

            var result = await itemsController.DeleteAsync(id);

            Assert.That(result is NotFoundObjectResult, Is.True);
        }

        [Test]
        public async Task ItemsController_DeleteItem_OK()
        {
            var id = Guid.NewGuid();
            itemsController = new ItemsController(itemServiceMock.Object, loggerMock.Object);

            var result = await itemsController.DeleteAsync(id);

            Assert.That(result is OkResult, Is.True);
        }
    }
}

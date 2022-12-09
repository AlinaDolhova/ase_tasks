using System;
using System.Collections.Generic;
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
    internal class ItemControllerTests
    {
        private ItemsController itemsController;
        private Mock<IItemService> itemServiceMock;
        private Mock<ILogger<ItemsController>> loggerMock;
        private Mock<IMapper> mapper;

        [SetUp]
        public void Setup()
        {
            this.loggerMock = new Mock<ILogger<ItemsController>>();
            this.itemServiceMock = new Mock<IItemService>();
            this.mapper = new Mock<IMapper>();
        }


        [Test]
        public async Task ItemsController_AddItem_OK()
        {
            var id = Guid.NewGuid();
            var name = "name";

            mapper.Setup(x => x.Map<Item>(It.IsAny<ItemInput>())).Returns<ItemInput>(x => new Item { Id = x.Id, CategoryId = x.CategoryId, Name = x.Name });

            itemsController = new ItemsController(itemServiceMock.Object, loggerMock.Object, mapper.Object);

            var result = await itemsController.AddAsync(new ItemInput { Id = id, Name = name });

            itemServiceMock.Verify(x => x.AddAsync(It.Is<Item>(x => x.Id == id && x.Name == name)));
        }


        [Test]
        public async Task ItemsController_UpdateItem_OK()
        {
            var id = Guid.NewGuid();
            var name = "name";

            mapper.Setup(x => x.Map<Item>(It.IsAny<ItemInput>())).Returns<ItemInput>(x => new Item { Id = x.Id, CategoryId = x.CategoryId, Name = x.Name });

            itemsController = new ItemsController(itemServiceMock.Object, loggerMock.Object, mapper.Object);

            var result = await itemsController.UpdateAsync(id, new ItemInput { Id = id, Name = name });

            itemServiceMock.Verify(x => x.UpdateAsync(id, It.Is<Item>(x => x.Id == id && x.Name == name)));
        }


        [Test]
        public async Task ItemsController_DeleteItem_NotFound()
        {
            var id = Guid.NewGuid();
            itemServiceMock.Setup(x => x.DeleteAsync(id)).Throws(new KeyNotFoundException());
            itemsController = new ItemsController(itemServiceMock.Object, loggerMock.Object, mapper.Object);

            var result = await itemsController.DeleteAsync(id);

            Assert.That(result, Is.EqualTo("Item was not deleted"));
        }

        [Test]
        public async Task ItemsController_DeleteItem_OK()
        {
            var id = Guid.NewGuid();
            itemsController = new ItemsController(itemServiceMock.Object, loggerMock.Object, mapper.Object);

            var result = await itemsController.DeleteAsync(id);

            Assert.That(result, Is.EqualTo("Item was deleted"));
        }
    }
}

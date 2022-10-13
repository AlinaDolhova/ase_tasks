using AutoMapper;
using CatalogService.BLL.Services;
using CatalogService.DAL.Interfaces;
using CatalogService.DAL.Models;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CatalogService.Tests
{
    public class ItemServiceTests
    {
        private Mock<IGenericRepository<Item>> repoMock;
        private BLL.Services.ItemService itemService;
        private Mock<IMapper> mapperMock;

        [SetUp]
        public void Setup()
        {
            repoMock = new Mock<IGenericRepository<Item>>();
            mapperMock = new Mock<IMapper>();

            mapperMock.Setup(x => x.Map<Model.Item>(It.IsAny<Category>())).Returns((Item x) => new Model.Item { Id = x.Id, Name = x.Name, ImageUrl = x.ImageUrl, Amount = x.Amount, Money = x.Money, CategoryId = x.CategoryId });
            mapperMock.Setup(x => x.Map<Item>(It.IsAny<Model.Item>())).Returns((Model.Item x) => new Item { Id = x.Id, Name = x.Name, ImageUrl = x.ImageUrl, Amount = x.Amount, Money = x.Money, CategoryId = x.CategoryId });

            mapperMock.Setup(x => x.Map(It.IsAny<Model.Item>(), It.IsAny<Item>())).Callback((Model.Item c1, Item c2) => c2.Name = c1.Name);
        }

        [Test]
        public async Task AddAsync_AddsItem()
        {
            var item = new Model.Item { Name = "test", CategoryId = Guid.NewGuid(), Money = 1, Amount = 1 };
            itemService = new ItemService(repoMock.Object, mapperMock.Object);
            await itemService.AddAsync(item);

            repoMock.Verify(x => x.AddAsync(It.Is<Item>(x => x.Name == item.Name)));
        }

        [Test]
        public void AddAsync_ValidatesName()
        {
            var item = new Model.Item();
            itemService = new ItemService(repoMock.Object, mapperMock.Object);
            Assert.ThrowsAsync<ArgumentException>(() => itemService.AddAsync(item));

        }

        [Test]
        public void AddAsync_ValidatesNullCategory()
        {
            itemService = new ItemService(repoMock.Object, mapperMock.Object);
            Assert.ThrowsAsync<ArgumentNullException>(() => itemService.AddAsync(null));
        }

    }
}
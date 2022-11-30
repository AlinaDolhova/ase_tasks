using AutoMapper;
using CartingService.API.Controllers;
using CartingService.BLL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;

namespace CartingService.Tests.API
{
    public class CartControllerTests
    {
        private Mock<ICartService> cartService;
        private Mock<IMapper> mapperMock;
        private CartController cartController;
        private Mock<ILogger<CartController>> loggerMock;

        [SetUp]
        public void Setup()
        {
            cartService = new Mock<ICartService>();
            mapperMock = new Mock<IMapper>();
            loggerMock = new Mock<ILogger<CartController>>();
        }

        [Test]
        public void Get_ReturnsCartModel_OK_V1()
        {
            var key = Guid.NewGuid();

            cartController = new CartController(cartService.Object, mapperMock.Object, loggerMock.Object);
            var result = cartController.Get(key);

            Assert.True(result.Result is OkObjectResult);
        }       

        [Test]
        public void Add_AddsCartItem_OK()
        {
            var key = Guid.NewGuid();

            cartController = new CartController(cartService.Object, mapperMock.Object, loggerMock.Object);
            var result = cartController.Add(key, new CartingService.API.Models.ItemModel());

            Assert.True(result is OkResult);
        }

        [Test]
        public void Delete_RemovesItemFromCart_OK()
        {
            var key = Guid.NewGuid();
            Guid itemId = Guid.NewGuid();

            cartController = new CartController(cartService.Object, mapperMock.Object, loggerMock.Object);
            var result = cartController.Delete(key, itemId);

            Assert.True(result is OkResult);
        }
    }
}

using AutoMapper;
using CartingService.API.Controllers;
using CartingService.BLL;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace CartingService.Tests.API
{
    public class CartControllerTests
    {
        private Mock<ICartService> cartService;
        private Mock<IMapper> mapperMock;
        private CartController cartController;

        [SetUp]
        public void Setup()
        {
            cartService = new Mock<ICartService>();
            mapperMock = new Mock<IMapper>();
        }

        [Test]
        public void Get_ReturnsCartModel_OK_V1()
        {
            string key = "1";

            cartController = new CartController(cartService.Object, mapperMock.Object);
            var result = cartController.Get(key);

            Assert.True(result.Result is OkObjectResult);
        }

        [Test]
        public void Get_ReturnsItemsList_OK_V2()
        {
            string key = "1";

            cartController = new CartController(cartService.Object, mapperMock.Object);
            var result = cartController.GetListWithItems(key);

            Assert.True(result.Result is OkObjectResult);
        }

        [Test]
        public void Add_AddsCartItem_OK()
        {
            string key = "1";

            cartController = new CartController(cartService.Object, mapperMock.Object);
            var result = cartController.Add(key, new CartingService.API.Models.ItemModel());

            Assert.True(result is OkResult);
        }

        [Test]
        public void Delete_RemovesItemFromCart_OK()
        {
            string key = "1";
            Guid itemId = Guid.NewGuid();

            cartController = new CartController(cartService.Object, mapperMock.Object);
            var result = cartController.Delete(key, itemId);

            Assert.True(result is OkResult);
        }
    }
}

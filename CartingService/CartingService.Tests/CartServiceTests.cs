using CartingService.BLL;
using CartingService.DAL.Interfaces;
using CartingService.Models;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CartingService.Tests
{
    
    public class CartServiceTests
    {
        private Mock<ICartRepository> cartRepo;
        private Mock<ILogger<CartService>> cartServiceLogger;
        private CartService cartingService;

        [SetUp]
        public void Setup()
        {
            cartRepo = new Mock<ICartRepository>();
            cartServiceLogger = new Mock<ILogger<CartService>>();

        }

        [Test]
        public void GetCartItems_ReturnsItemsFromExistingCart()
        {
            var cartId = Guid.NewGuid();
            var cart = new Cart { Id = cartId, CartItems = new List<CartItem> { new CartItem() } };

            cartRepo.Setup(x => x.GetCartById(cartId)).Returns(cart);

            cartingService = new CartService(cartRepo.Object, cartServiceLogger.Object);
            var result = cartingService.GetCartItems(cartId).ToList();

            Assert.That(result.Count, Is.EqualTo(1));
        }

        [Test]
        public void GetCartItems_ReturnsNullFromNotExistingCart()
        {
            var cartId = Guid.NewGuid();

            cartingService = new CartService(cartRepo.Object, cartServiceLogger.Object);
            var result = cartingService.GetCartItems(cartId);

            Assert.That(result, Is.Null);
        }

        [Test]
        public void AddItemToCart_AddsNewItemToCart()
        {
            var cartId = Guid.NewGuid();
            var item = new CartItem { Id = Guid.NewGuid(), Quantity = 1 };
            var cart = new Cart { Id = cartId, CartItems = new List<CartItem> { new CartItem() { Id = Guid.NewGuid(), Quantity = 1 } } };

            cartRepo.Setup(x => x.GetCartById(cartId)).Returns(cart);

            cartingService = new CartService(cartRepo.Object, cartServiceLogger.Object);
            cartingService.AddItemToCart(cartId, item);

            cartRepo.Verify(x => x.Upsert(It.Is<Cart>(cart => cart.CartItems.Count == 2 && cart.CartItems.All(cartItem => cartItem.Quantity == 1))));
        }

        [Test]
        public void AddItemToCart_UpdatesItemsCount()
        {
            var cartId = Guid.NewGuid();
            var item = new CartItem { Id = Guid.NewGuid() };
            var cart = new Cart { Id = cartId, CartItems = new List<CartItem> { new CartItem() { Id = item.Id, Quantity = 1 } } };

            cartRepo.Setup(x => x.GetCartById(cartId)).Returns(cart);

            cartingService = new CartService(cartRepo.Object, cartServiceLogger.Object);
            cartingService.AddItemToCart(cartId, item);

            cartRepo.Verify(x => x.Upsert(It.Is<Cart>(cart => cart.CartItems.Count == 1 && cart.CartItems.Single().Quantity == 2)));
        }

        [Test]
        public void AddItemToCart_CreatesNewCartIfThereIsNone()
        {
            var cartId = Guid.NewGuid();
            var item = new CartItem { Id = Guid.NewGuid() };

            cartingService = new CartService(cartRepo.Object, cartServiceLogger.Object);
            cartingService.AddItemToCart(cartId, item);

            cartRepo.Verify(x => x.Insert(It.Is<Cart>(cart => cart.Id == cartId && cart.CartItems.Count == 1 && cart.CartItems.Single().Quantity == 1)));
        }

        [Test]
        public void RemoveItemFromCart_DecreasesCountOfItemInCart()
        {
            var cartId = Guid.NewGuid();          
            var itemId = Guid.NewGuid();
            var cart = new Cart { Id = cartId, CartItems = new List<CartItem> { new CartItem() { Id = itemId, Quantity = 2 } } };

            cartRepo.Setup(x => x.GetCartById(cartId)).Returns(cart);

            cartingService = new CartService(cartRepo.Object, cartServiceLogger.Object);
            cartingService.RemoveItemFromCart(cartId, itemId);

            cartRepo.Verify(x => x.Upsert(It.Is<Cart>(cart => cart.CartItems.Count == 1 && cart.CartItems.Single().Quantity == 1)));
        }

        [Test]
        public void RemoveItemFromCart_RemovesItemFromCart()
        {
            var cartId = Guid.NewGuid();
            var remainingItemId = Guid.NewGuid();
            var itemId = Guid.NewGuid();
            var cart = new Cart { Id = cartId, CartItems = new List<CartItem> { new CartItem() { Id = itemId, Quantity = 1 }, new CartItem() { Id = remainingItemId, Quantity = 1 } } };

            cartRepo.Setup(x => x.GetCartById(cartId)).Returns(cart);

            cartingService = new CartService(cartRepo.Object, cartServiceLogger.Object);
            cartingService.RemoveItemFromCart(cartId, itemId);

            cartRepo.Verify(x => x.Upsert(It.Is<Cart>(cart => cart.CartItems.Count == 1 && cart.CartItems.Single().Id == remainingItemId)));
        }

        [Test]
        public void RemoveItemFromCart_RemovesCartIfItsEmpty()
        {
            var cartId = Guid.NewGuid();
            var itemId = Guid.NewGuid();
            var cart = new Cart { Id = cartId, CartItems = new List<CartItem> { new CartItem() { Id = itemId, Quantity = 1 } } };

            cartRepo.Setup(x => x.GetCartById(cartId)).Returns(cart);

            cartingService = new CartService(cartRepo.Object, cartServiceLogger.Object);
            cartingService.RemoveItemFromCart(cartId, itemId);

            cartRepo.Verify(x => x.Delete(It.Is<Guid>(id => id == cartId)));
        }

    }
}
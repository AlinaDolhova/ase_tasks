using CartingService.BLL;
using CartingService.DAL.Interfaces;
using CartingService.Models;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace CartingService.Tests
{
    public class CartServiceTests
    {
        private Mock<ICartRepository> cartRepo;
        private CartService cartingService;

        [SetUp]
        public void Setup()
        {
            cartRepo = new Mock<ICartRepository>();

        }

        [Test]
        public void GetCartItems_ReturnsItemsFromExistingCart()
        {
            var cartId = 1;
            var cart = new Cart { Id = cartId, CartItems = new List<CartItem> { new CartItem() } };

            cartRepo.Setup(x => x.GetCartById(cartId)).Returns(cart);

            cartingService = new CartService(cartRepo.Object);
            var result = cartingService.GetCartItems(cartId).ToList();

            Assert.That(result.Count, Is.EqualTo(1));
        }

        [Test]
        public void GetCartItems_ReturnsNullFromNotExistingCart()
        {
            var cartId = 1;

            cartingService = new CartService(cartRepo.Object);
            var result = cartingService.GetCartItems(cartId);

            Assert.That(result, Is.Null);
        }

        [Test]
        public void AddItemToCart_AddsNewItemToCart()
        {
            var cartId = 1;
            var item = new CartItem { Id = 2 };
            var cart = new Cart { Id = cartId, CartItems = new List<CartItem> { new CartItem() { Id = 3, Quantity = 1 } } };

            cartRepo.Setup(x => x.GetCartById(cartId)).Returns(cart);

            cartingService = new CartService(cartRepo.Object);
            cartingService.AddItemToCart(cartId, item);

            cartRepo.Verify(x => x.Upsert(It.Is<Cart>(cart => cart.CartItems.Count == 2 && cart.CartItems.All(cartItem => cartItem.Quantity == 1))));
        }

        [Test]
        public void AddItemToCart_UpdatesItemsCount()
        {
            var cartId = 1;
            var item = new CartItem { Id = 2 };
            var cart = new Cart { Id = cartId, CartItems = new List<CartItem> { new CartItem() { Id = item.Id, Quantity = 1 } } };

            cartRepo.Setup(x => x.GetCartById(cartId)).Returns(cart);

            cartingService = new CartService(cartRepo.Object);
            cartingService.AddItemToCart(cartId, item);

            cartRepo.Verify(x => x.Upsert(It.Is<Cart>(cart => cart.CartItems.Count == 1 && cart.CartItems.Single().Quantity == 2)));
        }

        [Test]
        public void AddItemToCart_CreatesNewCartIfThereIsNone()
        {
            var cartId = 1;
            var item = new CartItem { Id = 2 };

            cartingService = new CartService(cartRepo.Object);
            cartingService.AddItemToCart(cartId, item);

            cartRepo.Verify(x => x.Insert(It.Is<Cart>(cart => cart.Id == 1 && cart.CartItems.Count == 1 && cart.CartItems.Single().Quantity == 1)));
        }

        [Test]
        public void RemoveItemFromCart_DecreasesCountOfItemInCart()
        {
            var cartId = 1;          
            var itemId = 2;
            var cart = new Cart { Id = cartId, CartItems = new List<CartItem> { new CartItem() { Id = itemId, Quantity = 2 } } };

            cartRepo.Setup(x => x.GetCartById(cartId)).Returns(cart);

            cartingService = new CartService(cartRepo.Object);
            cartingService.RemoveItemFromCart(cartId, itemId);

            cartRepo.Verify(x => x.Upsert(It.Is<Cart>(cart => cart.CartItems.Count == 1 && cart.CartItems.Single().Quantity == 1)));
        }

        [Test]
        public void RemoveItemFromCart_RemovesItemFromCart()
        {
            var cartId = 1;
            var remainingItemId = 3;
            var itemId = 2;
            var cart = new Cart { Id = cartId, CartItems = new List<CartItem> { new CartItem() { Id = itemId, Quantity = 1 }, new CartItem() { Id = remainingItemId, Quantity = 1 } } };

            cartRepo.Setup(x => x.GetCartById(cartId)).Returns(cart);

            cartingService = new CartService(cartRepo.Object);
            cartingService.RemoveItemFromCart(cartId, itemId);

            cartRepo.Verify(x => x.Upsert(It.Is<Cart>(cart => cart.CartItems.Count == 1 && cart.CartItems.Single().Id == remainingItemId)));
        }

        [Test]
        public void RemoveItemFromCart_RemovesCartIfItsEmpty()
        {
            var cartId = 1;
            var itemId = 2;
            var cart = new Cart { Id = cartId, CartItems = new List<CartItem> { new CartItem() { Id = itemId, Quantity = 1 } } };

            cartRepo.Setup(x => x.GetCartById(cartId)).Returns(cart);

            cartingService = new CartService(cartRepo.Object);
            cartingService.RemoveItemFromCart(cartId, itemId);

            cartRepo.Verify(x => x.Delete(It.Is<int>(id => id == cartId)));
        }

    }
}
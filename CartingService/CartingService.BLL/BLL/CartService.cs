using CartingService.DAL.Interfaces;
using CartingService.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CartingService.BLL
{
    public class CartService : ICartService
    {
        private readonly ICartRepository cartRepository;
        private readonly ILogger<CartService> logger;

        public CartService(ICartRepository cartRepository, ILogger<CartService> logger)
        {
            this.cartRepository = cartRepository;
            this.logger = logger;
        }

        public void AddItemToCart(Guid cartId, CartItem item)
        {
            logger.LogDebug("CartService AddItemToCart in cart id {cartId} cart item {itemId}. Started", cartId, item.Id);

            if (item.Quantity == 0)
            {
                logger.LogDebug("CartService AddItemToCart for cart id {cartId} item {itemId} quantity is 0", cartId, item.Id);
                item.Quantity++;
            }

            var cart = cartRepository.GetCartById(cartId);
            if (cart != null)
            {
                var cartItem = cart.CartItems.FirstOrDefault(x => x.Id == item.Id);
                if (cartItem == null)
                {
                    logger.LogDebug("CartService AddItemToCart in cart id {cartId} cart item {itemId} is not presented.", cartId, item.Id);
                    // TODO: Get more info about item from another service? 
                    cart.CartItems.Add(item);
                }
                else
                {
                    cartItem.Quantity++;
                    logger.LogDebug("CartService AddItemToCart in cart id {cartId} cart item {itemId} exists. New quantity: {quantity}", cartId, item.Id, cartItem.Quantity);

                }

                logger.LogDebug("CartService AddItemToCart in cart id {cartId} cart item {itemId}. Upsert called", cartId, item.Id);

                cartRepository.Upsert(cart);
            }
            else
            {
                logger.LogDebug("CartService AddItemToCart in cart id {cartId} cart item {itemId}. Insert called", cartId, item.Id);
                cartRepository.Insert(new Cart { Id = cartId, CartItems = new List<CartItem> { item } });
            }

            logger.LogDebug("CartService AddItemToCart in cart id {cartId} cart item {itemId}. Completed", cartId, item.Id);
        }

        public IEnumerable<CartItem> GetCartItems(Guid cartId)
        {
            logger.LogDebug("CartService GetCartItems with cart id {cartId}. Started", cartId);

            var result = cartRepository.GetCartById(cartId)?.CartItems;

            logger.LogDebug("CartService GetCartItems with cart id {cartId}. Results count: {itemsCount}. Completed", cartId, result?.Count ?? 0);
            return result;
        }

        public void RemoveItemFromCart(Guid cartId, Guid itemId)
        {
            logger.LogDebug("CartService RemoveItemFromCart for cart id {cartId} cart item {itemId}. Started", cartId, itemId);

            var cart = cartRepository.GetCartById(cartId);
            if (cart != null)
            {
                var cartItem = cart.CartItems.FirstOrDefault(x => x.Id == itemId);
                if (cartItem != null)
                {
                    if (cartItem.Quantity > 1)
                    {
                        cartItem.Quantity--;
                        logger.LogDebug("CartService RemoveItemFromCart for cart id {cartId} cart item {itemId}. New count of items: {quantity}", cartId, itemId, cartItem.Quantity);

                    }
                    else
                    {
                        cart.CartItems.Remove(cartItem);
                        logger.LogDebug("CartService RemoveItemFromCart for cart id {cartId} cart item {itemId}. Cart item was removed", cartId, itemId);
                    }

                    if (!cart.CartItems.Any())
                    {
                        logger.LogDebug("CartService RemoveItemFromCart for cart id {cartId} cart item {itemId}. Cart was removed", cartId, itemId);
                        cartRepository.Delete(cartId);
                    }
                    else
                    {
                        logger.LogDebug("CartService RemoveItemFromCart for cart id {cartId} cart item {itemId}. Cart was upserted", cartId, itemId);
                        cartRepository.Upsert(cart);
                    }
                }
                else
                {
                    logger.LogWarning("CartService RemoveItemFromCart for cart id {cartId} cart item {itemId}. Cart item not found", cartId, itemId);
                }
            }
            else
            {
                logger.LogWarning("CartService RemoveItemFromCart for cart id {cartId} cart item {itemId}. Cart not found", cartId, itemId);
            }

            logger.LogDebug("CartService RemoveItemFromCart for cart id {cartId} cart item {itemId}. Completed", cartId, itemId);
        }

        public void UpdateItemInCarts(Guid itemId, string name, decimal price)
        {
            logger.LogDebug("CartService UpdateItemInCarts for item id {itemId}, new name {name}, new proce {price}. Started", itemId, name, price);

            var cartsWithItem = this.cartRepository.GetCartsByItemId(itemId);

            foreach (var cart in cartsWithItem)
            {
                var item = cart.CartItems.Where(x => x.Id == itemId).First();
                item.Price = price;
                item.Name = name;
            }

            logger.LogDebug("CartService UpdateItemInCarts for item id {itemId}, new name {name}, new proce {price}. Upsert {count} items", itemId, name, price, cartsWithItem.Count());
            this.cartRepository.UpsertAll(cartsWithItem);
            logger.LogDebug("CartService UpdateItemInCarts for item id {itemId}, new name {name}, new proce {price}. Completed", itemId, name, price);
        }
    }
}

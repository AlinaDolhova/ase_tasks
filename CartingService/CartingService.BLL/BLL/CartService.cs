using CartingService.DAL.Interfaces;
using CartingService.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CartingService.BLL
{
    public class CartService : ICartService
    {
        private readonly ICartRepository cartRepository;

        public CartService(ICartRepository cartRepository)
        {
            this.cartRepository = cartRepository;
        }

        public void AddItemToCart(Guid cartId, CartItem item)
        {
            if (item.Quantity == 0)
            {
                item.Quantity++;
            }

            var cart = cartRepository.GetCartById(cartId);
            if (cart != null)
            {
                var cartItem = cart.CartItems.FirstOrDefault(x => x.Id == item.Id);
                if (cartItem == null)
                {
                    // TODO: Get more info about item from another service? 
                    cart.CartItems.Add(item);
                }
                else
                {
                    cartItem.Quantity++;
                }

                cartRepository.Upsert(cart);
            }
            else
            {
                cartRepository.Insert(new Cart { Id = cartId, CartItems = new List<CartItem> { item } });
            }
        }

        public IEnumerable<CartItem> GetCartItems(Guid cartId)
        {
            return cartRepository.GetCartById(cartId)?.CartItems;
        }

        public void RemoveItemFromCart(Guid cartId, Guid itemId)
        {
            var cart = cartRepository.GetCartById(cartId);
            if (cart != null)
            {
                var cartItem = cart.CartItems.FirstOrDefault(x => x.Id == itemId);
                if (cartItem != null)
                {
                    if (cartItem.Quantity > 1)
                    {
                        cartItem.Quantity--;
                    }
                    else
                    {
                        cart.CartItems.Remove(cartItem);
                    }

                    if (!cart.CartItems.Any())
                    {
                        cartRepository.Delete(cartId);
                    }
                    else
                    {
                        cartRepository.Upsert(cart);
                    }
                }
            }
        }

        public void UpdateItemInCarts(Guid itemId, string name, decimal price)
        {
            var cartsWithItem = this.cartRepository.GetCartsByItemId(itemId);

            foreach(var cart in cartsWithItem)
            {
                var item = cart.CartItems.Where(x => x.Id == itemId).First();
                item.Price = price;
                item.Name = name;
            }

            this.cartRepository.UpsertAll(cartsWithItem);
        }
    }
}

using CartingService.Models;
using System;
using System.Collections.Generic;

namespace CartingService.BLL
{
    public interface ICartService
    {
        IEnumerable<CartItem> GetCartItems(Guid cartId);

        void AddItemToCart(Guid cartId, CartItem item);

        void RemoveItemFromCart(Guid cartId, Guid itemId);

        void UpdateItemInCarts(Guid itemId, string name, decimal price);
    }
}

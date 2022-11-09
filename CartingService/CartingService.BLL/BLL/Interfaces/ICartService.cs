using CartingService.Models;
using System;
using System.Collections.Generic;

namespace CartingService.BLL
{
    public interface ICartService
    {
        IEnumerable<CartItem> GetCartItems(int cartId);

        void AddItemToCart(int cartId, CartItem item);

        void RemoveItemFromCart(int cartId, Guid itemId);

        void UpdateItemInCarts(Guid itemId, string name, decimal price);
    }
}

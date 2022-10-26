using CartingService.Models;
using System.Collections.Generic;

namespace CartingService.BLL
{
    public interface ICartService
    {
        IEnumerable<CartItem> GetCartItems(int cartId);

        void AddItemToCart(int cartId, CartItem item);

        void RemoveItemFromCart(int cartId, int itemId);
    }
}

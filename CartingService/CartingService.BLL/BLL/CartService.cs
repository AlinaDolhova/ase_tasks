using CartingService.DAL.Interfaces;
using CartingService.Models;
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

        public void AddItemToCart(int cartId, CartItem item)
        {
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

        public IEnumerable<CartItem> GetCartItems(int cartId)
        {
            return cartRepository.GetCartById(cartId)?.CartItems;
        }

        public void RemoveItemFromCart(int cartId, int itemId)
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
    }
}

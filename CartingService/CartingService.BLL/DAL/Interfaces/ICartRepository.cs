using CartingService.Models;
using System;
using System.Collections.Generic;

namespace CartingService.DAL.Interfaces
{
    public interface ICartRepository
    {
        Cart GetCartById(int cartId);
        void Upsert(Cart cart);
        void Insert(Cart cart);

        void Delete(int cartId);

        IEnumerable<Cart> GetCartsByItemId(Guid itemId);

        void UpsertAll(IEnumerable<Cart> carts);
    }
}

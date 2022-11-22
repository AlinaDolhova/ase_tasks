using CartingService.Models;
using System;
using System.Collections.Generic;

namespace CartingService.DAL.Interfaces
{
    public interface ICartRepository
    {
        Cart GetCartById(Guid cartId);
        void Upsert(Cart cart);
        void Insert(Cart cart);

        void Delete(Guid cartId);

        IEnumerable<Cart> GetCartsByItemId(params Guid[] itemsId);

        void UpsertAll(IEnumerable<Cart> carts);
    }
}

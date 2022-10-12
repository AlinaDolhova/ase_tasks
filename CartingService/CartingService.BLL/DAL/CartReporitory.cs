using CartingService.DAL.Interfaces;
using CartingService.Models;
using LiteDB;
using System.Collections.Generic;
using System.Linq;

namespace CartingService.DAL
{
    public class CartReporitory : ICartRepository
    {
        private readonly ILiteCollection<Cart> cartCollection;

        public CartReporitory(ILiteDatabase db)
        {
            cartCollection = db.GetCollection<Cart>();
        }

        public Cart GetCartById(int cartId)
        {
            return cartCollection.Find(x => x.Id == cartId).FirstOrDefault();
        }

        public void Upsert(Cart cart)
        {
            cartCollection.Upsert(cart);
        }

        public void Insert(Cart cart)
        {
            cartCollection.Insert(cart);
        }

        public void Delete(int cartId)
        {
            cartCollection.DeleteMany(x => x.Id == cartId);
        }
    }
}

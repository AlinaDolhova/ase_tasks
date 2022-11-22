using CartingService.DAL.Interfaces;
using CartingService.Models;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CartingService.DAL
{
    public class CartReporitory : ICartRepository
    {
        private readonly ILiteCollection<Cart> cartCollection;
        private readonly ILiteDatabase liteDatabase;

        public CartReporitory(ILiteDatabase db)
        {
            cartCollection = db.GetCollection<Cart>();
            this.liteDatabase = db;
        }

        public Cart GetCartById(Guid cartId)
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

        public void Delete(Guid cartId)
        {
            cartCollection.DeleteMany(x => x.Id == cartId);
        }

        public IEnumerable<Cart> GetCartsByItemId(params Guid[] itemsId)
        {
            var results = liteDatabase.Execute
                ("select $ from Cart include CartItems where $.CartItems[*].Value any in @0",
                BsonMapper.Global.Serialize(itemsId)).ToList();

            return results.Select(x => BsonMapper.Global.Deserialize<Cart>(x));
        }

        public void UpsertAll(IEnumerable<Cart> carts)
        {
            liteDatabase.BeginTrans();
            try
            {
                carts.Select(x => cartCollection.Upsert(x));               
                liteDatabase.Commit();
            }
            catch
            {
                liteDatabase.Rollback();
                throw;
            }

        }
    }
}

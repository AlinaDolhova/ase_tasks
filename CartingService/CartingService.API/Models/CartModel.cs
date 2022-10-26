
using System.Collections.Generic;

namespace CartingService.API.Models
{
    public class CartModel
    {
        public string CartKey { get; set; }

        public IEnumerable<ItemModel> Items { get; set; }
    }
}

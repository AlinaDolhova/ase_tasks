using System;
using System.Collections.Generic;
using System.Text;

namespace CartingService.Models
{
    public class Cart
    {
        public int Id { get; set; }

        public IList<CartItem> CartItems { get; set; }
    }
}

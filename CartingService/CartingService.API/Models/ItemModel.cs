using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CartingService.API.Models
{
    public class ItemModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Image { get; set; }

        public decimal Price { get; set; }

        public int Quantity { get; set; }
    }
}

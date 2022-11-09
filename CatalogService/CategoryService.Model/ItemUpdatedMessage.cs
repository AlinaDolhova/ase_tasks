using System;
using System.Collections.Generic;
using System.Text;

namespace CatalogService.Model
{
    public class ItemUpdatedMessage
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public decimal Price { get; set; }
    }
}

using System;

namespace CatalogService.Model
{
    public class Item
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string ImageUrl { get; set; }

        public Guid CategoryId { get; set; }

        public decimal Money { get; set; }

        public int Amount { get; set; }

        public Category Category { get; set; }
    }
}

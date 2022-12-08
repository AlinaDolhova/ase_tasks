using System;

namespace CatalogService.GraphQL.Models
{
    public class ItemInput
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public Guid CategoryId { get; set; }

        public decimal Money { get; set; }

        public int Amount { get; set; }
    }
}

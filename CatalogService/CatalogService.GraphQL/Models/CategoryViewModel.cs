using System;
using System.Collections;
using System.Collections.Generic;

namespace CatalogService.GraphQL.Models
{
    public class CategoryViewModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string ImageUrl { get; set; }

        public Guid? ParentCategoryId { get; set; }

        public IEnumerable<ItemViewModel> Items { get; set; }
    }
}

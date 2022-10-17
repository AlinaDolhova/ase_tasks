using System;
using System.Collections.Generic;

namespace CatalogService.Model
{
    public class Category
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string ImageUrl { get; set; }

        public Guid? ParentCategoryId { get; set; }

        public IEnumerable<Guid> Items { get; set; }
    }
}

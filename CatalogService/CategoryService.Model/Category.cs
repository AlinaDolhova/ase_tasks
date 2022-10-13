using System;

namespace CatalogService.Model
{
    public class Category
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string ImageUrl { get; set; }

        public Guid? ParentCategoryId { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace CatalogService.GraphQL.Models
{
    public class CategoryInput
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

    }
}

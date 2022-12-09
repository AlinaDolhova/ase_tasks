using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using CatalogService.DAL.Interfaces;

namespace CatalogService.DAL.Models
{
    public class Category : IIdentifiable, IDeletable
    {
        [Required]
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        public string ImageUrl { get; set; }

        public Guid? ParentCategoryId { get; set; }

        public Category ParentCategory { get; set; }

        public bool IsDeleted { get; set; }

        public IEnumerable<Category> ChildCategories { get; set; }

        public IEnumerable<Item> Items { get; set; }
    }
}

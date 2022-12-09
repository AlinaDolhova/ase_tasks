using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using CatalogService.DAL.Interfaces;

namespace CatalogService.DAL.Models
{
    public class Item : IIdentifiable, IDeletable
    {
        [Key]
        [Required]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        public string Description { get; set; }

        [Url]
        public string ImageUrl { get; set; }

        [Required]
        public Guid CategoryId { get; set; }

        public Category Category { get; set; }

        [Required]
        [Column(TypeName = "money")]
        public decimal Money { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int Amount { get; set; }

        public bool IsDeleted { get; set; }
    }
}

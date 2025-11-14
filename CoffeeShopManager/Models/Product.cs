using System.ComponentModel.DataAnnotations;

namespace CoffeeShopManager.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }
        public string? ImagePath { get; set; }

        public int CategoryId { get; set; }
        public Category? Category { get; set; }
    }
}
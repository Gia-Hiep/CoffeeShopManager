using System.ComponentModel.DataAnnotations;

namespace CoffeeShopManager.Models
{
    public class Table
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string TableNumber { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Capacity { get; set; }

        public string Status { get; set; } // "Available", "Occupied"
    }
}
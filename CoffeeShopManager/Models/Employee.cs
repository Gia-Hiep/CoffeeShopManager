using System.ComponentModel.DataAnnotations;

namespace CoffeeShopManager.Models
{
    public class Employee
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [StringLength(50)]
        public string Position { get; set; } // "Waiter", "Barista", "Manager"

        [StringLength(100)]
        public string Email { get; set; }

        [StringLength(15)]
        public string Phone { get; set; }

        public ICollection<Shift>? Shifts { get; set; }
    }
}
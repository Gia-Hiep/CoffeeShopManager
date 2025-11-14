using System.ComponentModel.DataAnnotations;

namespace CoffeeShopManager.Models
{
    public class Order
    {
        public int Id { get; set; }

        [Required]
        public DateTime OrderDate { get; set; }

        public int TableId { get; set; }
        public Table? Table { get; set; }

        public int EmployeeId { get; set; }
        public Employee? Employee { get; set; }

        public List<OrderDetail>? OrderDetails { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? TotalAmount { get; set; }

        public string? Status { get; set; } // "Pending", "Completed", "Cancelled"
    }
}
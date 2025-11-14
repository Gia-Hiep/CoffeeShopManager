using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoffeeShopManager.Models
{
    public class Shift
    {
        public int Id { get; set; }

        [Required]
        public int EmployeeId { get; set; }
        public Employee? Employee { get; set; }

        [Required]
        [StringLength(50)]
        public string ShiftType { get; set; } // "Morning", "Afternoon", "Evening"

        [Required]
        public DateTime ShiftDate { get; set; } // Ngày của ca làm

        [StringLength(50)]
        public string Status { get; set; } // "Scheduled", "Completed"

        [Required]
        public DateTime StartTime { get; set; } // Lưu trực tiếp vào DB

        [Required]
        public DateTime EndTime { get; set; }   // Lưu trực tiếp vào DB

        [NotMapped]
        public TimeSpan Duration => EndTime - StartTime;

        [NotMapped]
        public TimeSpan? Overtime
        {
            get
            {
                var duration = EndTime - StartTime;
                var regularHours = TimeSpan.FromHours(5); // Ca cố định: 5 giờ
                return duration > regularHours ? duration - regularHours : TimeSpan.Zero;
            }
        }
    }
}
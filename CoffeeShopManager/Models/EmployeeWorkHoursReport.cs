namespace CoffeeShopManager.Models
{
    public class EmployeeWorkHoursReport
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public double TotalHours { get; set; }
        public decimal Salary { get; set; }
        public List<DateTime> ShiftDates { get; set; } = new List<DateTime>(); // Danh sách các ngày làm việc
    }
}
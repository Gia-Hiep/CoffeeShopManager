using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CoffeeShopManager.Data;
using CoffeeShopManager.Models;

namespace CoffeeShopManager.Pages.Shifts
{
    public class ReportsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public ReportsModel(ApplicationDbContext context) => _context = context;

        public IList<EmployeeWorkHoursReport> WorkHoursReport { get; set; } = new List<EmployeeWorkHoursReport>();
        public SelectList EmployeeList { get; set; }
        public IList<Shift> Shifts { get; set; } = new List<Shift>();

        [BindProperty(SupportsGet = true)]
        public string ReportType { get; set; } = "Weekly";

        [BindProperty(SupportsGet = true)]
        public DateTime? StartDate { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? SelectedEmployeeId { get; set; }

        public async Task OnGetAsync()
        {
            // Tạo danh sách nhân viên cho dropdown
            EmployeeList = new SelectList(
                await _context.Employees
                    .OrderBy(e => e.Name)
                    .Select(e => new { e.Id, e.Name })
                    .ToListAsync(),
                "Id", "Name", SelectedEmployeeId);

            // Xác định ngày bắt đầu và kết thúc
            var start = StartDate ?? (ReportType == "Weekly"
                ? DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek)
                : new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1));
            var end = ReportType == "Weekly" ? start.AddDays(6) : start.AddMonths(1).AddDays(-1);

            // Lấy danh sách ca làm việc
            var shiftQuery = _context.Shifts
                .Include(s => s.Employee)
                .Where(s => s.StartTime >= start && s.EndTime <= end && s.Status == "Completed");

            if (SelectedEmployeeId.HasValue)
                shiftQuery = shiftQuery.Where(s => s.EmployeeId == SelectedEmployeeId.Value);

            Shifts = await shiftQuery.ToListAsync();

            // Tạo báo cáo giờ làm việc
            WorkHoursReport = Shifts
                .GroupBy(s => s.EmployeeId)
                .Select(g => new EmployeeWorkHoursReport
                {
                    EmployeeId = g.Key,
                    EmployeeName = g.First().Employee?.Name ?? "Không xác định",
                    TotalHours = g.Sum(s => s.Duration.TotalHours),
                    ShiftDates = g.Select(s => s.ShiftDate.Date).Distinct().OrderBy(d => d).ToList(),
                    Salary = (decimal)(g.Sum(s => s.Duration.TotalHours) * 15000)
                })
                .OrderBy(r => r.EmployeeName)
                .ToList();

            // Thông báo nếu không có dữ liệu
            if (!WorkHoursReport.Any())
                TempData["Message"] = "Không có dữ liệu ca làm việc.";
        }
    }
}
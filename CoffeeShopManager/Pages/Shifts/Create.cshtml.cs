using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CoffeeShopManager.Data;
using CoffeeShopManager.Models;

namespace CoffeeShopManager.Pages.Shifts
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Shift Shift { get; set; }

        // Add ViewData property to replace ViewBag
        [ViewData]
        public SelectList EmployeeList { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            // Initialize default values for Shift
            Shift = new Shift
            {
                Status = "Scheduled",
                ShiftDate = DateTime.Today
            };

            // Load employee list and assign to ViewData property
            EmployeeList = new SelectList(
                await _context.Employees.ToListAsync(),
                "Id",
                "Name"
            );

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                // Reload employee list if validation fails
                EmployeeList = new SelectList(
                    await _context.Employees.ToListAsync(),
                    "Id",
                    "Name"
                );
                return Page();
            }

            // Calculate StartTime and EndTime based on ShiftType and ShiftDate
            var startTimeSpan = Shift.ShiftType switch
            {
                "Morning" => new TimeSpan(6, 0, 0),   // 06:00
                "Afternoon" => new TimeSpan(13, 0, 0), // 13:00
                "Evening" => new TimeSpan(17, 0, 0),  // 17:00
                _ => new TimeSpan(0, 0, 0)
            };

            var endTimeSpan = Shift.ShiftType switch
            {
                "Morning" => new TimeSpan(11, 0, 0),  // 11:00
                "Afternoon" => new TimeSpan(17, 0, 0), // 17:00
                "Evening" => new TimeSpan(22, 0, 0),  // 22:00
                _ => new TimeSpan(0, 0, 0)
            };

            Shift.StartTime = Shift.ShiftDate.Date + startTimeSpan;
            Shift.EndTime = Shift.ShiftDate.Date + endTimeSpan;

            _context.Shifts.Add(Shift);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
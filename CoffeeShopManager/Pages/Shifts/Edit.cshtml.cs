using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using CoffeeShopManager.Data;
using CoffeeShopManager.Models;

namespace CoffeeShopManager.Pages.Shifts
{
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Shift Shift { get; set; }

        public SelectList EmployeeList { get; set; } // Replace ViewBag with a property

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Shift = await _context.Shifts
                .Include(s => s.Employee)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (Shift == null)
            {
                return NotFound();
            }

            // Load the employee list into the EmployeeList property
            EmployeeList = new SelectList(
                await _context.Employees.ToListAsync(),
                "Id",
                "Name",
                Shift.EmployeeId
            );

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                // Reload the employee list if validation fails
                EmployeeList = new SelectList(
                    await _context.Employees.ToListAsync(),
                    "Id",
                    "Name",
                    Shift.EmployeeId
                );
                return Page();
            }

            var startTimeSpan = Shift.ShiftType switch
            {
                "Morning" => new TimeSpan(6, 0, 0),
                "Afternoon" => new TimeSpan(13, 0, 0),
                "Evening" => new TimeSpan(17, 0, 0),
                _ => new TimeSpan(0, 0, 0)
            };

            var endTimeSpan = Shift.ShiftType switch
            {
                "Morning" => new TimeSpan(11, 0, 0),
                "Afternoon" => new TimeSpan(17, 0, 0),
                "Evening" => new TimeSpan(22, 0, 0),
                _ => new TimeSpan(0, 0, 0)
            };

            Shift.StartTime = Shift.ShiftDate.Date + startTimeSpan;
            Shift.EndTime = Shift.ShiftDate.Date + endTimeSpan;

            _context.Attach(Shift).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Shifts.Any(e => e.Id == Shift.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using CoffeeShopManager.Data;
using CoffeeShopManager.Models;

namespace CoffeeShopManager.Pages.Employees
{
    public class CreateModel : PageModel
    {
        private readonly CoffeeShopManager.Data.ApplicationDbContext _context;

        public CreateModel(CoffeeShopManager.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            Employee = new Employee();
            ViewData["Position"] = new SelectList(new List<string> { "Waiter", "Barista", "Manager" }, Employee.Position);
            return Page();
        }

        [BindProperty]
        public Employee Employee { get; set; } = default!;

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                ViewData["Position"] = new SelectList(new List<string> { "Waiter", "Barista", "Manager" }, Employee.Position);
                return Page();
            }

            _context.Employees.Add(Employee);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
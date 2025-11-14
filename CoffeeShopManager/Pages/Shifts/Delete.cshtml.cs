using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CoffeeShopManager.Data;
using CoffeeShopManager.Models;

namespace CoffeeShopManager.Pages.Shifts
{
    public class DeleteModel : PageModel
    {
        private readonly CoffeeShopManager.Data.ApplicationDbContext _context;

        public DeleteModel(CoffeeShopManager.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Shift Shift { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shift = await _context.Shifts
                .Include(s => s.Employee) 
                .FirstOrDefaultAsync(m => m.Id == id);

            if (shift is not null)
            {
                Shift = shift;
                return Page();
            }

            return NotFound();
        }


        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shift = await _context.Shifts.FindAsync(id);
            if (shift != null)
            {
                Shift = shift;
                _context.Shifts.Remove(Shift);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}

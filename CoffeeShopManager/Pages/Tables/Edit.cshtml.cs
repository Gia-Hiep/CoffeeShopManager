using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CoffeeShopManager.Data;
using CoffeeShopManager.Models;

namespace CoffeeShopManager.Pages.Tables
{
    public class EditModel : PageModel
    {
        private readonly CoffeeShopManager.Data.ApplicationDbContext _context;

        public EditModel(CoffeeShopManager.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Table Table { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var table =  await _context.Tables.FirstOrDefaultAsync(m => m.Id == id);
            if (table == null)
            {
                return NotFound();
            }
            Table = table;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Update(Table);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}

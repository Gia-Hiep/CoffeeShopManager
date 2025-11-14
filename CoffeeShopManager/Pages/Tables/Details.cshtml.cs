using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CoffeeShopManager.Data;
using CoffeeShopManager.Models;

namespace CoffeeShopManager.Pages.Tables
{
    public class DetailsModel : PageModel
    {
        private readonly CoffeeShopManager.Data.ApplicationDbContext _context;

        public DetailsModel(CoffeeShopManager.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public Table Table { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var table = await _context.Tables.FirstOrDefaultAsync(m => m.Id == id);

            if (table is not null)
            {
                Table = table;

                return Page();
            }

            return NotFound();
        }
    }
}

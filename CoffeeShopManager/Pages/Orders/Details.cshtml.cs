using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CoffeeShopManager.Data;
using CoffeeShopManager.Models;

namespace CoffeeShopManager.Pages.Orders
{
    public class DetailsModel : PageModel
    {
        private readonly CoffeeShopManager.Data.ApplicationDbContext _context;

        public DetailsModel(CoffeeShopManager.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public Order Order { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return NotFound();

            Order = await _context.Orders
                .Include(o => o.Employee)
                .Include(o => o.Table)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (Order == null) return NotFound();

            return Page();
        }

    }
}

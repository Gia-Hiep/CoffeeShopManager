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
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Order> Order { get; set; } = default!;

        public async Task OnGetAsync()
        {
            Order = await _context.Orders
                .Include(o => o.Employee)
                .Include(o => o.Table)
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostCompleteOrderAsync(int id)
        {
            var order = await _context.Orders
                .Include(o => o.Table)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            // Cập nhật trạng thái đơn hàng thành "Completed"
            order.Status = "Completed";

            await _context.SaveChangesAsync();
            return RedirectToPage("./Index");
        }
    }
}
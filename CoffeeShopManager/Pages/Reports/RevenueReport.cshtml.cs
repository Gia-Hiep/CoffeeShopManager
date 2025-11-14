using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CoffeeShopManager.Data;
using CoffeeShopManager.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoffeeShopManager.Pages.Reports
{
    public class RevenueReportModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public RevenueReportModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Order> Orders { get; set; }
        public decimal TotalRevenue { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? StartDate { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? EndDate { get; set; }

        public async Task OnGetAsync()
        {
            var query = _context.Orders
                .Include(o => o.Table)
                .Include(o => o.Employee)
                .AsQueryable();

            if (StartDate.HasValue)
            {
                query = query.Where(o => o.OrderDate >= StartDate.Value);
            }

            if (EndDate.HasValue)
            {
                query = query.Where(o => o.OrderDate <= EndDate.Value.AddDays(1));
            }

            Orders = await query.OrderByDescending(o => o.OrderDate).ToListAsync();
            TotalRevenue = Orders.Sum(o => o.TotalAmount ?? 0);
        }
    }
}
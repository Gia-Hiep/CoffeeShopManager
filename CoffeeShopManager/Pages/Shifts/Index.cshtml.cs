using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CoffeeShopManager.Data;
using CoffeeShopManager.Models;

namespace CoffeeShopManager.Pages.Shifts
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Shift> Shift { get; set; }

        public async Task OnGetAsync()
        {
            Shift = await _context.Shifts
                .Include(s => s.Employee)
                .ToListAsync();
        }
    }
}
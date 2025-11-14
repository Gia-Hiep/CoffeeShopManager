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
    public class IndexModel : PageModel
    {
        private readonly CoffeeShopManager.Data.ApplicationDbContext _context;

        public IndexModel(CoffeeShopManager.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Table> Table { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Table = await _context.Tables.ToListAsync();
        }
    }
}

using CoffeeShopManager.Data;
using CoffeeShopManager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoffeeShopManager.ViewComponents
{
    public class ProductsViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public ProductsViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(int count)
        {
            var topProducts = await _context.OrderDetails
                .GroupBy(od => od.ProductId)
                .Select(g => new
                {
                    ProductId = g.Key,
                    TotalQuantity = g.Sum(od => od.Quantity)
                })
                .OrderByDescending(g => g.TotalQuantity)
                .Take(count)
                .Join(_context.Products,
                    g => g.ProductId,
                    p => p.Id,
                    (g, p) => new TopProductModel
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Price = p.Price,
                        ImagePath = p.ImagePath,
                        TotalQuantity = g.TotalQuantity
                    })
                .ToListAsync();

            return View(topProducts);
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using CoffeeShopManager.Data;
using CoffeeShopManager.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoffeeShopManager.Pages.Orders
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<SelectListItem> Products { get; set; } = new();

        [BindProperty]
        public Order Order { get; set; } = default!;

        [BindProperty]
        public List<OrderDetail> OrderDetails { get; set; } = new();

        public IActionResult OnGet()
        {
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "Name");
            ViewData["TableId"] = new SelectList(_context.Tables, "Id", "TableNumber");

            Products = _context.Products
                .Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = $"{p.Name} ({p.Price:N0} VNĐ)"
                }).ToList();

            OrderDetails.Add(new OrderDetail { Quantity = 0, UnitPrice = 0 });

            Order = new Order { OrderDate = DateTime.Now, TotalAmount = 0 };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "Name", Order.EmployeeId);
            ViewData["TableId"] = new SelectList(_context.Tables, "Id", "TableNumber", Order.TableId);

            Products = _context.Products
                .Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = $"{p.Name} ({p.Price:N0} VNĐ)"
                }).ToList();

            // Lọc các dòng hợp lệ 
            var validDetails = OrderDetails
                .Where(d => d.ProductId > 0 && d.Quantity > 0)
                .ToList();

            decimal total = 0;
            foreach (var detail in validDetails)
            {
                var product = await _context.Products.FindAsync(detail.ProductId);
                if (product != null)
                {
                    detail.UnitPrice = product.Price;
                    total += product.Price * detail.Quantity;
                }
            }
            Order.TotalAmount = total;

            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Gán OrderDetails hợp lệ
            Order.OrderDetails = validDetails;
            Order.Status = "Pending";

            var table = await _context.Tables.FindAsync(Order.TableId);
            if (table != null)
            {
                table.Status = "Occupied";
            }

            _context.Orders.Add(Order);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
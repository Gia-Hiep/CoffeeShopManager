using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CoffeeShopManager.Data;
using CoffeeShopManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoffeeShopManager.Pages.Orders
{
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Order Order { get; set; } = default!;

        [BindProperty]
        public List<OrderDetail> OrderDetails { get; set; } = new();

        public List<SelectListItem> Products { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return NotFound();

            var order = await _context.Orders
                .Include(o => o.Table)
                .Include(o => o.Employee)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (order == null) return NotFound();

            Order = order;
            OrderDetails = order.OrderDetails?.ToList() ?? new List<OrderDetail>();

            // Đảm bảo có ít nhất 5 dòng trống để người dùng có thể nhập
            while (OrderDetails.Count < 5)
            {
                OrderDetails.Add(new OrderDetail { Quantity = 0, UnitPrice = 0 });
            }

            Products = await _context.Products
                .Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = $"{p.Name} ({p.Price:N0} VNĐ)"
                }).ToListAsync();

            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "Name", Order.EmployeeId);
            ViewData["TableId"] = new SelectList(_context.Tables, "Id", "TableNumber", Order.TableId);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "Name", Order.EmployeeId);
            ViewData["TableId"] = new SelectList(_context.Tables, "Id", "TableNumber", Order.TableId);

            Products = await _context.Products
                .Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = $"{p.Name} ({p.Price:N0} VNĐ)"
                }).ToListAsync();

            // Lọc các dòng hợp lệ (ProductId > 0 và Quantity > 0)
            var validDetails = OrderDetails
                .Where(d => d.ProductId > 0 && d.Quantity > 0)
                .ToList();

            // Tính TotalAmount
            var productDict = await _context.Products
                .ToDictionaryAsync(p => p.Id, p => p.Price);
            decimal total = 0;

            foreach (var detail in validDetails)
            {
                if (productDict.TryGetValue(detail.ProductId, out var price))
                {
                    detail.UnitPrice = price;
                    total += price * detail.Quantity;
                }
            }
            Order.TotalAmount = total;

            // Kiểm tra ModelState và đảm bảo có ít nhất một dòng hợp lệ
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (!validDetails.Any())
            {
                ModelState.AddModelError("", "Please add at least one valid product with a quantity greater than 0.");
                return Page();
            }

            // Lấy đơn hàng gốc và thông tin bàn cũ
            var orderToUpdate = await _context.Orders
                .Include(o => o.Table)
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(o => o.Id == Order.Id);

            if (orderToUpdate == null) return NotFound();

            int oldTableId = orderToUpdate.TableId;
            string oldStatus = orderToUpdate.Status;

            // Cập nhật thông tin đơn hàng
            orderToUpdate.OrderDate = Order.OrderDate;
            orderToUpdate.TotalAmount = Order.TotalAmount;
            orderToUpdate.EmployeeId = Order.EmployeeId;
            orderToUpdate.TableId = Order.TableId;
            orderToUpdate.Status = Order.Status;

            // Xóa OrderDetails cũ và thêm OrderDetails mới
            _context.OrderDetails.RemoveRange(orderToUpdate.OrderDetails);
            orderToUpdate.OrderDetails = validDetails;
            foreach (var detail in orderToUpdate.OrderDetails)
            {
                detail.OrderId = orderToUpdate.Id;
            }

            // Nếu người dùng đổi bàn, cập nhật trạng thái bàn cũ về Available
            if (oldTableId != Order.TableId)
            {
                var oldTable = await _context.Tables.FindAsync(oldTableId);
                if (oldTable != null)
                {
                    oldTable.Status = "Available";
                }
            }

            // Cập nhật trạng thái bàn mới
            var newTable = await _context.Tables.FindAsync(Order.TableId);
            if (newTable != null)
            {
                newTable.Status = (Order.Status == "Cancelled") ? "Available" : "Occupied";
            }

            await _context.SaveChangesAsync();
            return RedirectToPage("./Index");
        }
    }
}
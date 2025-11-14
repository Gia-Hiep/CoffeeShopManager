using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CoffeeShopManager.Data;
using CoffeeShopManager.Models;
using Rotativa.AspNetCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CoffeeShopManager.Pages
{
    public class BillByTableModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public BillByTableModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public SelectList TableList { get; set; }
        public Table SelectedTable { get; set; }
        public int SelectedTableId { get; set; }
        public IList<Order> Orders { get; set; } = new List<Order>();
        public decimal TotalBill { get; set; }
        [TempData]
        public string PaymentMessage { get; set; }
        [TempData]
        public bool PaymentSuccess { get; set; }

        public async Task OnGetAsync(int? tableId)
        {
            var tables = await _context.Tables.ToListAsync();
            TableList = new SelectList(tables, "Id", "TableNumber", tableId);

            if (tableId.HasValue)
            {
                SelectedTableId = tableId.Value;
                SelectedTable = await _context.Tables.FindAsync(tableId.Value);

                Orders = await _context.Orders
                    .Include(o => o.Employee)
                    .Where(o => o.TableId == tableId.Value && (o.Status == "Pending" || o.Status == "Completed"))
                    .OrderByDescending(o => o.OrderDate) 
                    .ToListAsync();

                TotalBill = Orders.Sum(o => o.TotalAmount ?? 0);
            }
        }

        public async Task<IActionResult> OnPostAsync(int tableId)
        {
            if (tableId <= 0)
            {
                return NotFound();
            }

            // Lấy tất cả đơn hàng của bàn 
            var orders = await _context.Orders
                .Where(o => o.TableId == tableId)
                .ToListAsync();

            if (!orders.Any()) 
            {
                return RedirectToPage("./BillByTable");
            }

            foreach (var order in orders)
            {
                order.Status = "Cancelled";
            }

            // Cập nhật trạng thái bàn thành "Available" 
            var table = await _context.Tables.FindAsync(tableId);
            if (table != null)
            {
                table.Status = "Available";
            }

            await _context.SaveChangesAsync();
            PaymentMessage = "Thanh toán thành công!";
            PaymentSuccess = true;
            return RedirectToPage("./BillByTable");
        }

        public IActionResult OnGetPrintBill(int tableId)
        {
            var orders = _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .Where(o => o.TableId == tableId && (o.Status == "Pending" || o.Status == "Completed"))
                .ToList();

            if (!orders.Any())
            {
                return NotFound();
            }

            decimal totalBill = orders.Sum(o => o.TotalAmount ?? 0);
            var table = _context.Tables.Find(tableId);

            return new ViewAsPdf("PrintBill", new { Orders = orders, TotalBill = totalBill, TableNumber = table?.TableNumber })
            {
                FileName = $"Bill_Table_{tableId}.pdf",
                PageSize = Rotativa.AspNetCore.Options.Size.A4,
                PageMargins = new Rotativa.AspNetCore.Options.Margins { Left = 10, Right = 10, Top = 10, Bottom = 10 }
            };
        }
    }
}
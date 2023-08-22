using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Teashop2.Data;
using Teashop2.Models;

namespace Teashop2.Controllers
{
    public class OrderController : Controller
    {
        private readonly TeashopContext _context;

        public OrderController(TeashopContext context)
        {
            _context = context;
        }

        // GET: Order
        [Authorize(Policy = "writepolicy")]
        public async Task<IActionResult> Index()
        {
            var teashopContext = _context.Orders.Include(o => o.Shipment).Where(o => o.Status != Status.SHIPPED);

            return View(await teashopContext.ToListAsync());
        }

        // GET: Order/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Shipment)
                .Include(op => op.OrderedProducts)
                .ThenInclude(p => p.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            ViewData["Statuses"] = new SelectList(Enum.GetValues(typeof(Status)));

            return View(order);
        }

        [Authorize(Policy = "writepolicy")]
        public IActionResult Edit(int id, Status status)
        {
            Order order = _context.Orders.FirstOrDefault(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            order.Status = status;
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
          return (_context.Orders?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}

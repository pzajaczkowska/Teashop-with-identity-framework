using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DinkToPdf.Contracts;
using DinkToPdf;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Teashop2.Data;
using Teashop2.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Text;

namespace Teashop2.Controllers
{
    public class OrderController : Controller
    {
        private readonly TeashopContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IServiceProvider _serviceProvider;
        private readonly IConverter _pdfConverter;

        public OrderController
            (
                TeashopContext context,
                UserManager<IdentityUser> userManager,
                IServiceProvider serviceProvider,
                IConverter pdfConverter
            )
        {
            _context = context;
            _userManager = userManager;
            _serviceProvider = serviceProvider;
            _pdfConverter = pdfConverter;
        }

        // GET: Order
        [Authorize(Policy = "writepolicy")]
        public async Task<IActionResult> Index()
        {
            var teashopContext = _context.Orders.Include(o => o.Shipment).Where(o => o.Status != Status.SHIPPED);

            return View(await teashopContext.ToListAsync());
        }

        [Authorize(Roles = "User")]
        public async Task<IActionResult> UserData()
        {
            IdentityUser user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound("User not found");
            }

            var orders = _context.Orders.Include(o => o.Shipment).Where(o => o.User.Id == user.Id).OrderByDescending(o => o.Date);

            return View(await orders.ToListAsync());
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

        public IActionResult GeneratePdf(int orderId)
        {
            var order = _context.Orders.Include(o => o.OrderedProducts).ThenInclude(o => o.Product).FirstOrDefault(o => o.Id == orderId);

            if (order == null)
            {
                return NotFound();
            }

            var html = GetHtmlContent(order);

            var pdf = _pdfConverter.Convert(new HtmlToPdfDocument()
            {
                GlobalSettings = {
                ColorMode = ColorMode.Color,
                Orientation = Orientation.Portrait,
                PaperSize = PaperKind.A4
            },
                Objects = {
                new ObjectSettings
                {
                    PagesCount = true,
                    HtmlContent = html,
                    WebSettings = { DefaultEncoding = "utf-8" },
                }
            }
            });

            return File(pdf, "application/pdf", $"zamówienie{order.Date}.pdf");
        }

        private string GetHtmlContent(Order order)
        {
            string html = $@"
            <h1>Zamówienie nr {order.Id}</h1>
            <p>Data złożenia zamówienia: {order.Date}</p>
            <p>Adres wysyłki: {order.Address}</p>
            <p>Koszt całkowity: {order.TotalCost:c2}</p>
            <hr>
            <h2>Zamówione produkty</p>
            <table>
            <thead><tr><th>Produkt</th><th>Ilość</th><th>Cena jednostkowa [zł]</th><th>Wartość</th></tr></thead><tbody>
            ";

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(html);
            foreach (OrderedProduct p in order.OrderedProducts)
            {
                stringBuilder.Append($"<tr><td>{p.Product.Name}</td><td>{p.Quantity}</td><td>{p.PricePerUnit:c2}</td><td>{p.Quantity * p.PricePerUnit:c2}</td></tr>");
            }
            stringBuilder.Append("</tbody></table>");

           
            return stringBuilder.ToString();
        }

        private bool OrderExists(int id)
        {
          return (_context.Orders?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}

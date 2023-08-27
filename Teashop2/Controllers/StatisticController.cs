using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Teashop2.Data;
using Teashop2.Models;
using Teashop2.ViewModel;

namespace Teashop2.Controllers
{
    [Authorize(Policy = "writepolicy")]
    public class StatisticController : Controller
    {
        private readonly TeashopContext _context;
        private readonly UserManager<IdentityUser> _userManager;


        public StatisticController(TeashopContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {

            return View();
        }

        public async Task<IActionResult> Products()
        {
            var orderedProducts = _context.OrderedProducts.ToList();

            var productSales = orderedProducts.GroupBy(op => op.ProductId)
                                               .Select(group => new StatisticViewModel
                                               {
                                                   Key = _context.Products.FirstOrDefault(p => p.Id == group.Key).Name,
                                                   Value = group.Sum(op => op.Quantity).ToString()
                                               })
                                               .OrderBy(e => e.Key)
                                               .ToList();

            return View(productSales);
        }

        public async Task<IActionResult> Users()
        {
            var orders = await _context.Orders
                                        .Include(p => p.User)
                                        .ToListAsync();

            var statistics = orders.GroupBy(order => order.User.Id)
                                   .Select(group => new StatisticViewModel
                                   {
                                       Key = _userManager.FindByIdAsync(group.Key).Result.UserName,
                                       Value = group.Count().ToString()
                                   })
                                   .OrderBy(e => e.Key)
                                   .ToList();

            return View(statistics);
        }

    }
}
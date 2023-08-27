using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Teashop2.Data;
using Teashop2.ViewModel;
using Teashop2.Views.Statistic;

namespace Teashop2.Controllers
{
    [Authorize(Policy = "writepolicy")]
    public class StatisticController : Controller
    {
        private readonly TeashopContext _context;

        public StatisticController(TeashopContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var orderedProducts = _context.OrderedProducts.ToList();

            var productSales = orderedProducts.GroupBy(op => op.ProductId)
                                               .Select(group => new StatisticViewModel
                                               {
                                                   //Key = group.Key.ToString(),
                                                   Key = _context.Products.FirstOrDefault(p => p.Id == group.Key).Name,
                                                   Value = group.Sum(op => op.Quantity).ToString()
                                               })
                                               .ToList();

            return View(productSales);
        }
    }
}
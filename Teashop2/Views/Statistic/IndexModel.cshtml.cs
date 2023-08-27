using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Linq;
using Teashop2.Data;

namespace Teashop2.Views.Statistic;

public class IndexModel : PageModel
{
    private readonly TeashopContext _context;

    public IndexModel(TeashopContext context)
    {
        _context = context;
    }

    public void OnGet()
    {
        var orderedProducts = _context.OrderedProducts.ToList();

        var productSales = orderedProducts.GroupBy(op => op.ProductId)
                                           .Select(group => new
                                           {
                                               ProductId = group.Key,
                                               TotalQuantitySold = group.Sum(op => op.Quantity)
                                           })
                                           .ToList();

        ViewData["StatisticData"] = productSales;
    }
}

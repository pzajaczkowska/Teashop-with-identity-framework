using Microsoft.AspNetCore.Mvc;
using Teashop2.Data;
using Teashop2.Models;

namespace Teashop2.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly ShoppingCart _shoppingCart;
        private readonly TeashopContext _context;


        public ShoppingCartController(ShoppingCart shoppingCart, TeashopContext context)
        {
            _shoppingCart = shoppingCart;
            _context = context;
        }

        public IActionResult Index()
        {
            return View(_shoppingCart.Products);
        }

        public IActionResult AddToCart(int productId)
        {
            OrderedProduct product = _shoppingCart.Products.FirstOrDefault(x => x.ProductId == productId);

            if (product != null)
               product.Quantity += 1;
            else
            {
                Product actualProduct = _context.Products.FirstOrDefault(p => p.Id == productId);

                OrderedProduct productToAdd = new OrderedProduct {ProductId = productId, Product = actualProduct, Quantity = 1 };
                _shoppingCart.Products.Add(productToAdd);
            }

            return RedirectToAction("Index");
        }

        public IActionResult RemoveFromCart(int productId)
        {
            OrderedProduct product = _shoppingCart.Products.FirstOrDefault(x => x.ProductId == productId);

            if (product != null)
            {
                _shoppingCart.Products.Remove(product);
            }

            return RedirectToAction("Index");
        }

    }
}

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Teashop2.Areas.Identity.Data;
using Teashop2.Data;
using Teashop2.Models;
using Teashop2.ViewModels;

namespace Teashop2.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly ShoppingCart _shoppingCart;
        private readonly TeashopContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ShoppingCartController(
            ShoppingCart shoppingCart, 
            TeashopContext context,
            UserManager<IdentityUser> userManager
        )
        {
            _shoppingCart = shoppingCart;
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            List<OrderedProduct> copy = new List<OrderedProduct>(_shoppingCart.Products);

            foreach (OrderedProduct item in copy)
            {
                int quantityOnStock = _context.Products.FirstOrDefault(p => p.Id == item.Product.Id).QuantityOnStock;

                if (quantityOnStock == 0)
                    _shoppingCart.Products.Remove(item);
                else if (quantityOnStock < item.Quantity)
                    item.Quantity = quantityOnStock;
            }

            return View(_shoppingCart.Products);
        }

        public IActionResult AddToCart(int productId)
        {
            OrderedProduct product = _shoppingCart.Products.FirstOrDefault(x => x.Product.Id == productId);

            if (product != null)
               product.Quantity += 1;
            else
            {
                Product actualProduct = _context.Products.FirstOrDefault(p => p.Id == productId);

                OrderedProduct productToAdd = new OrderedProduct {Product = actualProduct, Quantity = 1 };
                _shoppingCart.Products.Add(productToAdd);
            }

            //return RedirectToAction("Index");
            return RedirectToAction("Details", "Product", new { id = productId });
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

        public IActionResult Checkout()
        {
            var viewModel = new CheckoutViewModel
            {
                Order = new Order(),
                Shipments = _context.Shipments.ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Checkout([Bind("PostalCode, City, Street, HouseNo, ShipmentId")]  Order order)
        {
            IdentityUser user = _userManager.GetUserAsync(User).Result;

            foreach (var item in _shoppingCart.Products)
            {
                item.PricePerUnit = item.Product.Price;
                item.ProductId = item.Product.Id;
                order.OrderedProducts.Add(item);
            }

            order.User = user;
            //order.UserId = user.Id;
            order.Shipment = _context.Shipments.FirstOrDefault(s => s.Id == order.ShipmentId);
            order.Status = Status.PAID;
            order.Date = DateTime.Now;
            order.TotalCost = _shoppingCart.Products.Sum(x => x.Quantity * x.Product.Price) + order.Shipment.Price;

            _context.Orders.Add(order);
            _context.UpdateProductsAfterCheckout(_shoppingCart.Products);
            _context.SaveChanges();

            _shoppingCart.ClearCart();

            return RedirectToAction("Index", "Home");
        }

    }
}

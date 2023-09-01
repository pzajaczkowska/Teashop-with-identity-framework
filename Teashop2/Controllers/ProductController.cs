using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging;
using Org.BouncyCastle.Asn1.Crmf;
using Teashop2.Data;
using Teashop2.Models;
using Teashop2.ViewModel;

namespace Teashop2.Controllers
{
    public class ProductController : Controller
    {
        private readonly TeashopContext _context;

        public ProductController(TeashopContext context)
        {
            _context = context;
        }


        public async Task<IActionResult> Index(
            string sortOrder, 
            string currentFilter,
            string searchString,
            int? pageNumber,
            int? categoryFilter
            )
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["PriceSortParm"] = sortOrder == "Price" ? "price_desc" : "Price";
            ViewData["categoryFilter"] = categoryFilter;
            ViewBag.Categories = await _context.Categories.ToListAsync();

            if (searchString != null)
                pageNumber = 1;
            else
                searchString = currentFilter;

            ViewData["CurrentFilter"] = searchString;


            var products = from p in _context.Products
                           select p;

            if (categoryFilter.HasValue)
            {
                products = products.Where(p => p.Categories.Any(c => c.Id == categoryFilter));
            }

            if (!String.IsNullOrEmpty(searchString))
            {
                products = products.Where(p => p.Name.ToUpper().Contains(searchString.ToUpper())
                                       || p.Description.ToUpper().Contains(searchString.ToUpper()))
                                    .Include(p => p.Categories);

            }

            switch (sortOrder)
            {
                case "name_desc":
                    products = products.OrderByDescending(p => p.IsAvaliable).ThenByDescending(p => p.Name);
                    break;
                case "Price":
                    products = products.OrderByDescending(p => p.IsAvaliable).ThenBy(p => p.Price);
                    break;
                case "price_desc":
                    products = products.OrderByDescending(p => p.IsAvaliable).ThenByDescending(p => p.Price);
                    break;
                default:
                    products = products.OrderByDescending(p => p.IsAvaliable).ThenBy(p => p.Name);
                    break;
            }

            int pageSize = 8;
            return View(await PaginatedList<Product>.CreateAsync(products.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Categories)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        [Authorize(Policy = "writepolicy")]
        public IActionResult Create()
        {
            var viewModel = new AddProductViewModel
            {
                Product = new Product(),
                Categories = new List<CategoryViewModel>()
            };

            foreach (Category cat in _context.Categories)
                viewModel.Categories.Add(
                        new CategoryViewModel
                        {
                            Id = cat.Id,
                            Name = cat.Name
                        }
                    );

            return View(viewModel);
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Policy = "writepolicy")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Product, Categories")] AddProductViewModel model, IFormFile imageFile)
        {
            try
            {
                var selectedCategories = model.Categories.Where(cat => cat.IsSelected).Select(cat => cat.Id);

                if (selectedCategories.Any())
                {
                    var product = new Product
                    {
                        Name = model.Product.Name,
                        Description = model.Product.Description,
                        Price = model.Product.Price,
                        QuantityOnStock = model.Product.QuantityOnStock,
                        IsAvaliable = model.Product.IsAvaliable,
                        Categories = _context.Categories.Where(cat => selectedCategories.Contains(cat.Id)).ToList(),
                        ImagePath = await SaveImage(imageFile)
                    };

                    _context.Add(product);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Unable to save changes. " +
                    "Try again, and if the problem persists, " +
                    "see your system administrator.");
            }
            return View(model);
        }

        // GET: Products/Edit/5
        [Authorize(Policy = "writepolicy")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                                            .Include(p => p.Categories)
                                            .AsNoTracking()
                                            .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            var viewModel = new AddProductViewModel
            {
                Product = product,
                Categories = new List<CategoryViewModel>(),
                PreviousImagePath = product.ImagePath
            };

            foreach (Category cat in _context.Categories)
            {
                var categoryViewModel = new CategoryViewModel
                {
                    Id = cat.Id,
                    Name = cat.Name
                };

                if (product.Categories.Any(selectedCat => selectedCat.Id == cat.Id))
                {
                    categoryViewModel.IsSelected = true;
                }

                viewModel.Categories.Add(categoryViewModel);
            }

            return View(viewModel);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Policy = "writepolicy")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Product, Categories, PreviousImagePath")] AddProductViewModel model, IFormFile? imageFile)
        {
            try
            {
                if (id != model.Product.Id)
                {
                    return NotFound();
                }

                var selectedCategories = model.Categories.Where(cat => cat.IsSelected).Select(cat => cat.Id);

                if (selectedCategories.Any())
                {
                    Product product = _context.Products
                                    .Include(p => p.Categories)
                                    .FirstOrDefault(p => p.Id == id);

                    product.Name = model.Product.Name;
                    product.Description = model.Product.Description;
                    product.Price = model.Product.Price;
                    product.QuantityOnStock = model.Product.QuantityOnStock;
                    product.IsAvaliable = model.Product.IsAvaliable;

                    if (imageFile != null)
                    {
                        product.ImagePath = await SaveImage(imageFile);
                    }
                    else if (!string.IsNullOrEmpty(model.PreviousImagePath))
                    {
                        product.ImagePath = model.PreviousImagePath;
                    }

                    var selectedCategoryIds = model.Categories.Where(cat => cat.IsSelected).Select(cat => cat.Id).ToList();
                    var categories = _context.Categories.Where(cat => selectedCategoryIds.Contains(cat.Id)).ToList();

                    product.Categories.Clear();
                    product.Categories.AddRange(categories);
                    _context.Products.Update(product);
                    _context.SaveChanges();

                    return RedirectToAction(nameof(Index));

                }

                else
                    return View(model);
            }

            catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(model.Product.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }            
        }

        private bool ProductExists(int id)
        {
          return (_context.Products?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        private async Task<string> SaveImage(IFormFile imageFile)
        {
            if (imageFile != null && imageFile.Length > 0)
            {
                var imageName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", imageName);

                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                return "/images/" + imageName;
            }
            return null;
        }
    }
}

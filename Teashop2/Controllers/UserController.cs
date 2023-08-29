

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Differencing;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging;
using Teashop2.Data;
using Teashop2.Models;
using Teashop2.ViewModel;

namespace Teashop2.Controllers
{
    [Authorize (Roles = "Admin")]
    public class UserController : Controller
    {
        RoleManager<IdentityRole> _roleManager;
        UserManager<IdentityUser> _userManager;
        private readonly TeashopContext _context;


        public UserController(
            RoleManager<IdentityRole> roleManager, 
            UserManager<IdentityUser> userManager,
            TeashopContext teashopContext
        )
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _context = teashopContext;
        }

        public async Task<IActionResult> Index()
        {
            List<UserWithRoles> usersWithRoles = await GetAllUsersWithRolesAsync();
            usersWithRoles = usersWithRoles.OrderBy(u => string.Join(",", u.Roles.OrderBy(r => r))).ToList();

            return View(usersWithRoles);
        }

        public async Task<IActionResult> Edit(string? id)
        {
            var toEdit = _userManager.FindByIdAsync(id).Result;
            if (id == null || toEdit == null)
            {
                return NotFound();
            }


            var assignedRoles = _userManager.GetRolesAsync(toEdit).Result;
            
            UserViewModel viewModel = new UserViewModel();
            viewModel.User = toEdit;
            viewModel.Roles = new List<RoleViewModel>();

            _roleManager.Roles.ForEachAsync(r => viewModel.Roles.Add(new RoleViewModel { Id = r.Id, Name = r.Name, IsSelected = assignedRoles.Contains(r.Name) }));


            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("User, Roles")] UserViewModel model)
        {
            if (id != model.User.Id)
            {
                return NotFound();
            }

            var selectedRoles = model.Roles.Where(r => r.IsSelected).Select(r => r.Name).ToList();

            if (selectedRoles.Any())
            {
                var user = _userManager.FindByIdAsync(id).Result;
                var assignedRoles = _userManager.GetRolesAsync(user).Result;

                await _userManager.RemoveFromRolesAsync(user, assignedRoles);
                await _userManager.AddToRolesAsync(user, selectedRoles);

                return RedirectToAction(nameof(Index));
            }

            else
                return View(model);
        }

        public  Task<List<UserWithRoles>> GetAllUsersWithRolesAsync()
        {
            var usersWithRoles =  _context.Users
                .Select(user => new UserWithRoles
                {
                    User = user,
                    Roles = _userManager.GetRolesAsync(user).Result
                })
                .ToListAsync();

            return usersWithRoles;
        }

        [AcceptVerbs("GET", "POST")]
        public IActionResult IsUserNameUnique(string userName)
        {
            var user = _userManager.FindByNameAsync(userName).Result;

            if (user == null)
            {
                return Json(true);
            }
            else
            {
                return Json("Login jest już zajęty.");
            }
        }

        [AcceptVerbs("GET", "POST")]
        public IActionResult IsEmailUnique(string email)
        {
            var user = _userManager.FindByEmailAsync(email).Result;

            if (user == null)
            {
                return Json(true);
            }
            else
            {
                return Json("Emails jest już zajęty.");
            }
        }
    }
}

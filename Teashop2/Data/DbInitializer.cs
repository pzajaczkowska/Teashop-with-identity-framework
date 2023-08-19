using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Teashop2.Areas.Identity.Data;
using Teashop2.Data;

namespace Teashop.Data
{
    public class DbInitializer
    {
        public static async Task Initialize(
            TeashopContext context, 
            UserManager<IdentityUser> userManager, 
            RoleManager<IdentityRole> roleManager
            )
        {
            context.Database.EnsureCreated();

            if (!context.Roles.Any())
            {

                var adminRole = "Admin";
                var roleNames = new String[] { adminRole, "Manager", "User" };

                foreach (var roleName in roleNames)
                {
                    var role = await roleManager.RoleExistsAsync(roleName);
                    if (!role)
                    {
                        await roleManager.CreateAsync(new IdentityRole { Name = roleName });
                    }
                }

                // administrator
                var admin = new ApplicationUser
                {
                    UserName = "admin",
                    Email = "admin@localhost.com",
                    EmailConfirmed = true
                };
                var existingAdmin = await userManager.FindByEmailAsync(admin.Email);

                if (existingAdmin == null)
                {
                    var adminUser = await userManager.CreateAsync(admin, "HasloMaslo12#");
                    if (adminUser.Succeeded)
                    {
                        await userManager.AddToRoleAsync(admin, adminRole);
                    }
                }

                // superuser
                var superuser = new ApplicationUser
                {
                    UserName = "superuser",
                    Email = "superuser@localhost.com",
                    EmailConfirmed = true
                };
                var existingSuperuser = await userManager.FindByEmailAsync(superuser.Email);

                if (existingSuperuser == null)
                {
                    var superuserUser = await userManager.CreateAsync(superuser, "HasloMaslo12#");
                    if (superuserUser.Succeeded)
                    {
                        await userManager.AddToRoleAsync(superuser, adminRole);
                        await userManager.AddToRoleAsync(superuser, "User");
                    }
                }

                // user
                var user = new ApplicationUser
                {
                    UserName = "user",
                    Email = "user@localhost.com",
                    EmailConfirmed = true
                };
                var existingUser = await userManager.FindByEmailAsync(user.Email);

                if (existingUser == null)
                {
                    var userUser = await userManager.CreateAsync(user, "HasloMaslo12#");
                    if (userUser.Succeeded)
                    {
                        await userManager.AddToRoleAsync(user, "User");
                    }
                }

                context.SaveChanges();
            }
        }
    }
}

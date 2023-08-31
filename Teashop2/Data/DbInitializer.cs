using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Teashop2.Areas.Identity.Data;
using Teashop2.Data;
using Teashop2.Models;

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

            if(!context.Categories.Any())
            {
                List<Category> categories = new List<Category>
            {
                new Category{Name = "Herbata czarna" },
                new Category{Name = "Herbata zielona" },
                new Category{Name = "Zioła" },
                new Category{Name = "Na dobry sen" },
                new Category{Name = "Yerba mate" },
                new Category{Name = "Herbatka owocowa" },
                new Category{Name = "Inne kolory herbaty" },
            };
                categories.ForEach(c => context.Categories.Add(c));
                context.SaveChanges();
            }

            if(!context.Products.Any())
            {
                List<Category> categories = context.Categories.ToList();
                List<Product> products = new List<Product>()
            {
                    new Product{
                    Name = "Afrykański słoń",
                    Description = "Wyśmienita kompozycja najlepszej cejlońskiej czarnej herbaty i owoców tropikalnych. Słodycz owoców doskonale łączy się z przyjemną goryczką herbaty, nadając jej przyjemnie orzeźwiający smak.",
                    Price = 19.99f,
                    QuantityOnStock = 10,
                    IsAvaliable = true,
                    Categories = {categories[0]},
                    ImagePath = "/images/herbata-saga.png"
                    },
                    new Product{
                        Name = "Zozole",
                        Description = "Dzięki połączeniu lekkiej japońskiej herbaty i cytrusów, zozole jest idealną opcją na lato. Wyśmienicie smakuje również na zimno.",
                        Price = 20.99f,
                        QuantityOnStock = 0,
                        IsAvaliable = false,
                        Categories = {categories[1]},
                        ImagePath = null
                    },
                    new Product{
                        Name = "Czarna magia",
                        Description = "Napar o barwie tak głębokiej, jak sugeruje nazwa, za sprawą cejlońskiej herbaty ze Sri Lanki i kwiatu hibiskusa. Owoc dzikiej róży uszlachetnia smak, zaś płatki kwiatów wprowadzają element magiczny.",
                        Price = 25.00f,
                        QuantityOnStock = 10,
                        IsAvaliable = true,
                        Categories = {categories[0]},
                        ImagePath = null
                    },
                    new Product{
                        Name = "Zielnik",
                        Description = "Jest to kompozycja ziół, która wyciszy skołatane nerwy czy nastroi organizm do snu.",
                        Price = 15.50f,
                        QuantityOnStock = 10,
                        IsAvaliable = true,
                        Categories = {categories[2], categories[3]},
                        ImagePath = null
                    },
                    new Product{
                        Name = "Uroda",
                        Description = "Wszystko, co jest potrzebne dla pięknej skóry, włosów i paznokci!",
                        Price = 12.00f,
                        QuantityOnStock = 10,
                        IsAvaliable = false,
                        Categories = {categories[2]}
                    },
                    new Product{
                        Name = "Blue lagoon",
                        Description = "Popularny drink w wersji bezalkoholowej i na ciepło. Klitoria oraz chaber odpowiadają za charakterystyczną barwę, zaś dodatki owocowe za smak.",
                        Price = 22.00f,
                        QuantityOnStock = 10,
                        IsAvaliable = true,
                        Categories = {categories[2]}
                    },
                    new Product{
                        Name = "Iq",
                        Description = "Dzięki dodatkowi skórki pomarańczy, mieszanka cechuje się niezwykłą świeżością. Korzenne ziołowe nuty w fuzji z owocami tworzą doskonale zbilansowane połączenie.",
                        Price = 18.00f,
                        QuantityOnStock = 10,
                        IsAvaliable = true,
                        Categories = {categories[2], categories[4]}
                    },
                    new Product{
                        Name = "Grzaniec",
                        Description = "Idealna propozycja na jesienne i zimowe wieczory. Korzenny aromat rozgrzeje zmarznięte ciało, a wyobraźnię przeniesie w wygodny fotel nieopodal iskrzącego kominka.",
                        Price = 19.00f,
                        QuantityOnStock = 10,
                        IsAvaliable = true,
                        Categories = {categories[0]}
                    },
                    new Product{
                        Name = "Z miłością",
                        Description = "Owocowa mieszanka o uniwersalnym smaku.  Słodko – kwaśne połączenie przełamane nutą mięty przypadnie każdemu do gustu.",
                        Price = 19.50f,
                        QuantityOnStock = 10,
                        IsAvaliable = true,
                        Categories = {categories[5]}
                    },
                    new Product{
                        Name = "Indie",
                        Description = "Herbata korzenna na bazie cejlońskich listów. Mieszanka bardzo rozgrzewająca – idealna na zimę, przy leczeniu przeziębienia czy na rozgrzanie dla zmarźluchów.",
                        Price = 22.99f,
                        QuantityOnStock = 10,
                        IsAvaliable = true,
                        Categories = {categories[0]}
                    },
                    new Product{
                        Name = "Leśny dzban",
                        Description = "Bardzo słodki napar o apetycznej barwie głębokiego borda. Dzięki naturalnej słodyczy malin, jagód i truskawek herbata nie wymaga słodzenia, a co za tym idzie, jest zdrowsza i mniej kaloryczna.",
                        Price = 17.00f,
                        QuantityOnStock = 10,
                        IsAvaliable = true,
                        Categories = {categories[5]}
                    },
                    new Product{
                        Name = "Biała mewa",
                        Description = "Najdelikatniejsza z naszych herbat. Biała herbata nie dominuje nad owocowymi aromatami, a idealnie z nimi współgra.",
                        Price = 28.00f,
                        QuantityOnStock = 10,
                        IsAvaliable = true,
                        Categories = {categories[6]}
                    },
                    new Product{
                        Name = "Relaks",
                        Description = "Tak jak nazwa mówi – idealna opcja dla poszukujących wypoczynku. Melisa i lawenda idealnie wyciszają, zaś pozostałe dodatki nadają naparowi charakterystyczny, lekko owocowy smak.",
                        Price = 20.00f,
                        QuantityOnStock = 10,
                        IsAvaliable = true,
                        Categories = {categories[2], categories[3]}
                    },
                    new Product{
                        Name = "Zdrówko",
                        Description = "Wszystko to, co dodajemy do herbaty na przeziębienie – miód, cytryna i maliny. ",
                        Price = 12.00f,
                        QuantityOnStock = 0,
                        IsAvaliable = false,
                        Categories = {categories[0]}
                    },
                    new Product{
                        Name = "I love you",
                        Description = "Klasyczna herbata o niebanalnych dodatkach w postaci romantycznych płatków róży i uroczych cukrowych serduszek – idealny prezent dla ukochanej osoby.",
                        Price = 19.99f,
                        QuantityOnStock = 10,
                        IsAvaliable = true,
                        Categories = {categories[0]}
                    },
                    new Product{
                        Name = "Słodkie szepty",
                        Description = "Bardzo słodkie połączenie składników, które szepnie Ci do ucha, że nie potrzebujesz więcej cukru.",
                        Price = 16.99f,
                        QuantityOnStock = 10,
                        IsAvaliable = true,
                        Categories = {categories[5]}
                    },
                    new Product{
                        Name = "Love story",
                        Description = "Lekkość senchy, słodycz malin i urok płatków róży wprowadzi Ciebie i Twoją sympatię w dobry nastrój.",
                        Price = 21.99f,
                        QuantityOnStock = 10,
                        IsAvaliable = true,
                        Categories = {categories[1]}
                    },
                    new Product{
                        Name = "Dobre pomarańczowe",
                        Description = "Bardzo dobra herbata o mocnym pomarańczowym aromacie!",
                        Price = 18.50f,
                        QuantityOnStock = 10,
                        IsAvaliable = true,
                        Categories = {categories[1]}
                    },
                    new Product{
                        Name = "Łąka",
                        Description = "Kwiatowy napar o bazie z japońskiej zielonej herbaty przywodzi na myśl letnią łąkę.",
                        Price = 14.50f,
                        QuantityOnStock = 10,
                        IsAvaliable = false,
                        Categories = {categories[2]}
                    },
                    new Product{
                        Name = "Masala chai",
                        Description = "To herbata z charakterem – dzięki korzennym przyprawom uzyskuje się zdecydowany, ale bardzo przyjemny napar o orientalnym aromacie. Można ją pić z dodatkiem mleka, osłodzoną cukrem lub miodem – otrzymamy wtedy przepyszny i pożywny napój.",
                        Price = 18.99f,
                        QuantityOnStock = 10,
                        IsAvaliable = true,
                        Categories = {categories[0]}
                    },
                    new Product{
                        Name = "Muminki",
                        Description = "Wyborne połączenie rooibosa i smakowitych owoców przeniesie Cię do świata z lat dzieciństwa – zaczarowanej krainy muminków!",
                        Price = 21.99f,
                        QuantityOnStock = 10,
                        IsAvaliable = true,
                        Categories = {categories[2]}
                    },
                    new Product{
                        Name = "Cukiereczek",
                        Description = "Kompozycja o smaku popularnych cukierków ziołowych Ricola, dobra na kaszel i odporność.",
                        Price = 18.00f,
                        QuantityOnStock = 0,
                        IsAvaliable = false,
                        Categories = {categories[2]}
                    },
                    new Product{
                        Name = "Pralina",
                        Description = "Wyborne połączenie rooibosa, cytrusowych nut, głębokiej barwy hibiskusa i smaku deserowej czekolady Lindt. Połączenie tak wytworne, jak najdroższe pralinki!",
                        Price = 29.99f,
                        QuantityOnStock = 10,
                        IsAvaliable = true,
                        Categories = {categories[2]}
                    }
                };

                products.ForEach(p => context.Products.Add(p));
                context.SaveChanges();
            }

            if(!context.Shipments.Any())
            {
                List<Shipment> shipments = new List<Shipment>()
            {
                new Shipment{Name = "Paczkomat InPost", Price = 13.99f, EstimatedTime = "1-2 dni roboczych"},
                new Shipment{Name = "Kurier Dpd", Price = 12.99f, EstimatedTime = "2-3 dni roboczych"},
                new Shipment{Name = "Orlen Paczka", Price = 4.5f, EstimatedTime = "3-5 dni roboczych"},
            };
                shipments.ForEach(s => context.Shipments.Add(s));
                context.SaveChanges();
            }
        }
    }
}

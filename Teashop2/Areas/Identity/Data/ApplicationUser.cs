using Microsoft.AspNetCore.Identity;

namespace Teashop2.Areas.Identity.Data
{
    public class ApplicationUser : IdentityUser
    {
        [PersonalData]
        public string FirstName { get; set; }

        [PersonalData]
        public String LastName { get; set; }
    }
}

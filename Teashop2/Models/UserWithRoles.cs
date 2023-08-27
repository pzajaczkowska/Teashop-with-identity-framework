using Microsoft.AspNetCore.Identity;

namespace Teashop2.Models
{
    public class UserWithRoles
    {
        public IdentityUser User { get; set; }
        public IList<string> Roles { get; set; }
    }
}

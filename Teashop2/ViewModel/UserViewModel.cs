using Microsoft.AspNetCore.Identity;

namespace Teashop2.ViewModel
{
    public class UserViewModel
    {
        public IdentityUser User { get; set; }
        public List<RoleViewModel> Roles { get; set; }
    }
}

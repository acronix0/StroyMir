using Microsoft.AspNetCore.Identity;
namespace SimpleShop.Core.Model   
{
    public class ApplicationUser : IdentityUser
    {
        public string DisplayName { get; set; }
        public bool Blocked { get; set; }
    }
}

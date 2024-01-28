using Microsoft.AspNetCore.Identity;

namespace DwitterSocial.Data
{
    public class AppRole : IdentityRole<int>
    {
        public ICollection<AppUserRole> UserRoles { get; set; }
    }
}
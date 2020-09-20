using Microsoft.AspNetCore.Identity;

namespace Isotope.Data.Models
{
    public class AppUser: IdentityUser
    {
        /// <summary>
        /// Flag indicating that the user has admin privileges.
        /// </summary>
        public bool IsAdmin { get; set; }
    }
}

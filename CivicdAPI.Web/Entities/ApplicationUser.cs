using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CivicdAPI.Web.Entities
{
    public class ApplicationUser: IdentityUser
    {
        public ApplicationUser()
        {
            this.Tags = new List<Tag>();
        }

        public async Task<IdentityResult> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager, string authenticationType)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateAsync(this, authenticationType);
            // Add custom user claims here
            return userIdentity;
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DisplayName { get; set; }
        public string ProfileDescription { get; set; }
        public string Photo { get; set; }
        public bool Verified { get; set; }
        public OrganizationCategory Category { get; set; }
        public LegalStatus LegalStatus { get; set; }
        public virtual Address Address { get; set; }

        public virtual ICollection<Tag> Tags { get; set; }
        public virtual ICollection<UserActivity> UserActivities { get; set; }
        public async Task<IdentityResult> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateAsync(this);
            // Add custom user claims here
            return userIdentity;
        }
    }
}
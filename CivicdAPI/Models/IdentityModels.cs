using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CivicdAPI.Models
{
  // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
  public class ApplicationUser : IdentityUser
  {
    public ApplicationUser()
    {
      this.Tags = new List<Tag>();
    }

    public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager, string authenticationType)
    {
      // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
      var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
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
    public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
    {
      // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
      var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
      // Add custom user claims here
      return userIdentity;
    }
  }

  public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
  {
    public ApplicationDbContext()
        : base("DefaultConnection", throwIfV1Schema: false)
    {
    }

    public static ApplicationDbContext Create()
    {
      return new ApplicationDbContext();
    }

    public DbSet<Activity> Activities { get; set; }
    public DbSet<UserActivity> UserActivities { get; set; }
    public DbSet<Address> Addresses { get; set; }
    public DbSet<Tag> Tags { get; set; }
  }
}
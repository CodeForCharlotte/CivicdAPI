using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CivicdAPI.Web.Entities
{
    public class ApplicationUser
    {
        public ApplicationUser()
        {
            this.Tags = new List<Tag>();
        }
        
        [Key]
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DisplayName { get; set; }
        public string ProfileDescription { get; set; }
        public string Photo { get; set; }
        public bool Verified { get; set; }
        public OrganizationCategory Category { get; set; }
        public LegalStatus LegalStatus { get; set; }
        public Address Address { get; set; }
        public virtual string Email { get; set; }
        public virtual bool EmailConfirmed { get; set; }
        public virtual string PasswordHash { get; set; }
        public virtual string SecurityStamp { get; set; }
        public virtual string PhoneNumber { get; set; }
        public virtual bool PhoneNumberConfirmed { get; set; }
        public virtual bool TwoFactorEnabled { get; set; }
        public virtual DateTime? LockoutEndDateUtc { get; set; }
        public virtual bool LockoutEnabled { get; set; }
        public virtual int AccessFailedCount { get; set; }
        public virtual string UserName { get; set; }

        public virtual ICollection<Tag> Tags { get; set; }
        public virtual ICollection<UserActivity> UserActivities { get; set; }
    }
}
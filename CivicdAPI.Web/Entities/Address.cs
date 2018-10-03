using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CivicdAPI.Web.Entities
{
    public class Address
    {
        public Address()
        {
            this.Users = new HashSet<ApplicationUser>();
            this.Activities = new HashSet<Activity>();
        }
        [Key]
        public int ID { get; set; }
        public string Name { get; set; }
        public string StreetAddressOne { get; set; }
        public string StreetAddressTwo { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public virtual ICollection<ApplicationUser> Users { get; set; }
        public virtual ICollection<Activity> Activities { get; set; }
    }
}
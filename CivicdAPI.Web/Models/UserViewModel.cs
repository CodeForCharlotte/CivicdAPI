using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CivicdAPI.Web.Models
{
    public class UserViewModel
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        public string DisplayName { get; set; }
        public string ProfileDescription { get; set; }
        public string StreetAddressOne { get; set; }
        public string StreetAddressTwo { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }

        public IEnumerable<TagDTO> Tags { get; set; }
    }
}
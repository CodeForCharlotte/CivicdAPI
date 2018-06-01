using CivicdAPI.Models.DTO;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CivicdAPI.Models
{
  public class UserDTO
  {
    [Required]
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string DisplayName { get; set; }
    public string OldPassword { get; set; }
    public string NewPassword { get; set; }
    [Required]
    public int Category { get; set; }
    public string PhoneNumber { get; set; }
    public string ProfileDescription { get; set; }
    public string StreetAddressOne { get; set; }
    public string StreetAddressTwo { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string ZipCode { get; set; }
    public IEnumerable<TagDTO> Tags { get; set; }
  }
}
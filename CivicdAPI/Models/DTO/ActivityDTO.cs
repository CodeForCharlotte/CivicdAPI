using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CivicdAPI.Models.DTO
{
  public class ActivityDTO
  {
    public ActivityDTO()
    {
      this.Tags = new HashSet<TagDTO>();
    }

    public int Id { get; set; }
    [Required]
    public string DisplayTitle { get; set; }
    public string Description { get; set; }
    public int Category { get; set; }
    public string PhotoURL { get; set; }
    [Required]
    public string StartTime { get; set; }
    public string EndTime { get; set; }
    public string AddressDisplayName { get; set; }
    public string StreetAddressOne { get; set; }
    public string StreetAddressTwo { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string ZipCode { get; set; }
    public string OrganizationEmail { get; set; }

    public UserDTO Organization { get; set; }
    public IEnumerable<TagDTO> Tags { get; set; }
  }
}
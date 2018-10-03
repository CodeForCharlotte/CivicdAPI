using System.ComponentModel.DataAnnotations;

namespace CivicdAPI.Web.Entities
{
    public enum LegalStatus
    {
        [Display(Name = "None or N/A")]
        NA,
        [Display(Name = "501(c)(3)")]
        Nonprofit
    }
}
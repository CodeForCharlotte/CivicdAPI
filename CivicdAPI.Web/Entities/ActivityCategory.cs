using System.ComponentModel.DataAnnotations;

namespace CivicdAPI.Web.Entities
{
    public enum ActivityCategory
    {
        [Display(Name = "Rally/Protest")]
        Protest,
        [Display(Name = "School Meeting")]
        School,
        [Display(Name = "Government Meeting")]
        Government,
        [Display(Name = "Internal Organization Meeting")]
        Internal,
        [Display(Name = "Informational Meeting")]
        Informational,
        [Display(Name = "Community Meeting")]
        Community,
        [Display(Name = "Independent Activity")]
        IndependentActivity
    }
}
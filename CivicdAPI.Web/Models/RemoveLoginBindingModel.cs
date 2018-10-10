﻿using CivicdAPI.Web.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CivicdAPI.Web.Models
{
    public class RemoveLoginBindingModel
    {
        [Required]
        [Display(Name = "User")]
        public ApplicationUser User { get; set; }

        [Required]
        [Display(Name = "Login provider")]
        public string LoginProvider { get; set; }

        [Required]
        [Display(Name = "Provider key")]
        public string ProviderKey { get; set; }
    }
}

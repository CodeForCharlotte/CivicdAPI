﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CivicdAPI.Models
{
  public class Tag
  {
    public Tag()
    {
      this.Activities = new HashSet<Activity>();
      this.Users = new HashSet<ApplicationUser>();
    }

    public Tag(string name)
    {
      this.Name = name;
      this.Activities = new HashSet<Activity>();
      this.Users = new HashSet<ApplicationUser>();
    }

    [Key]
    public int ID { get; set; }
    public string Name { get; set; }
    public virtual ICollection<Activity> Activities { get; set; }
    public virtual ICollection<ApplicationUser> Users { get; set; }
  }
}
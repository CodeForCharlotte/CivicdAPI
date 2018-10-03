using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CivicdAPI.Web.Models
{
    public class UserInfoViewModel
    {
        public string Email { get; set; }
        public bool HasRegistered { get; set; }
        public string LoginProvider { get; set; }
    }
}

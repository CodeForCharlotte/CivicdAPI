using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CivicdAPI.Web.Models
{
    public class LoginModel
    {
        public string Grant_Type { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}

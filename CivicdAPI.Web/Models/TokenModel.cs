using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace CivicdAPI.Web.Models
{
    public class TokenModel
    {
        public string Username { get; set; }
        public string Password { get; set; }

        [DisplayName("grant_type")]
        public string GrantType { get; set; }
    }
}

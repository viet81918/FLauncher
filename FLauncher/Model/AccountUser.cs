using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FLauncher.Model
{
    public class AccountUser
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Username { get; set; }
        public bool IsOnline { get; set; }
    }
}

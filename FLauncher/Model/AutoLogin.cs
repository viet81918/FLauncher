using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FLauncher.Model
{
    public class AutoLogin
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public DateTime ExpirationDate { get; set; }
    }
}

using MongoDB.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FLauncher.Model
{
    [Collection("User")]
    public class AccountUser
    {
        public string Email { get; set; }
        public string Password { get; set; }
        //public string Username { get; set; }
        //public bool IsOnline { get; set; }
        //public decimal Money { get; set; }
        //public decimal wallet { get; set; }

    }
}

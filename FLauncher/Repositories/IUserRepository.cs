using FLauncher.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FLauncher.Repositories
{
    public interface IUserRepository
    {
       List<User> GetUsers();
       User GetUserByEmailPass(string Email, string Pass);
       User GetUserByEmail(string Email);
    }
}

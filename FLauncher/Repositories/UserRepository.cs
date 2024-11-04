using FLauncher.DAO;
using FLauncher.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FLauncher.Repositories
{
    public class UserRepository : IUserRepository
    {
        public User GetUserByEmailPass(string Email, string Pass)
        {
            return  UserDAO.Instance.GetUserByEmailPass(Email, Pass);
        }

        public  List<User> GetUsers()
        {
            return  UserDAO.Instance.GetUsers();
        }
    }
}

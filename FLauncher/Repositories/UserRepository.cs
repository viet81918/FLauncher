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
        public async Task<User> GetUserByEmailPass(string Email, string Pass)
        {
            return await UserDAO.Instance.GetUserByEmailPass(Email, Pass);
        }

        public async Task<IEnumerable<User>> GetUsers()
        {
            return await UserDAO.Instance.GetUsers();
        }
    }
}

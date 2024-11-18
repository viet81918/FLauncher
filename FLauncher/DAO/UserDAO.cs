using FLauncher.Model;
using FLauncher.Services;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;

namespace FLauncher.DAO
{
    public class UserDAO : SingletonBase<UserDAO>
    {
        private readonly FlauncherDbContext _dbContext;

        public UserDAO()
        {
            var connectionString = "mongodb://localhost:27017/";
            var client = new MongoClient(connectionString);
            _dbContext = FlauncherDbContext.Create(client.GetDatabase("FPT"));
        }

        // Retrieve all users
        public List<User> GetUsers()
        {
            return  _dbContext.Users.ToList();
        }

        // Retrieve a user by email and password
        public User GetUserByEmailPass(string email, string pass)
        {
            return _dbContext.Users.First(c => c.Email.Equals(email) && c.Password.Equals(pass));
        }

        public User GetUserByEmail(string email)
        {
            return _dbContext.Users.First(c => c.Email.Equals(email));
        }
        public User GetUserByGamer(Gamer gamer)
        {
            return _dbContext.Users.First(c => c.ID == gamer.GamerId);
        }
    }
}

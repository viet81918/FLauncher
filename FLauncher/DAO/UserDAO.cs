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
        private readonly IMongoCollection<User> _userCollection;

        public UserDAO()
        {
            // Initialize MongoDB client and retrieve the "Users" collection
            var client = new MongoClient("mongodb://localhost:27017/");
            var database = client.GetDatabase("FPT");
            _userCollection = database.GetCollection<User>("Users");
        }

        // Retrieve all users
        public async Task<IEnumerable<User>> GetUsers()
        {
            // Use MongoDB's LINQ support to retrieve all users
            return await _userCollection.Find(_ => true).ToListAsync();
        }

        // Retrieve a user by email and password
        public async Task<User> GetUserByEmailPass(string email, string pass)
        {
            // Define filters for email and password
            var filter = Builders<User>.Filter.And(
                Builders<User>.Filter.Eq(u => u.Email, email),
                Builders<User>.Filter.Eq(u => u.Password, pass)
            );

            return await _userCollection.Find(filter).FirstOrDefaultAsync();
        }
    }

}

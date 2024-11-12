using FLauncher.Model;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using FLauncher.Services;
using Microsoft.EntityFrameworkCore;

namespace FLauncher.DAO
{
    public class GamerDAO : SingletonBase<GamerDAO>
    {
        private readonly FlauncherDbContext _dbContext;
        private readonly MongoDbContext _mongoDbContext;

        public GamerDAO()
        {
            var connectionString = "mongodb://localhost:27017/";
            var client = new MongoClient(connectionString);
            _dbContext = FlauncherDbContext.Create(client.GetDatabase("FPT"));
            _mongoDbContext = new MongoDbContext();
        }
        public Gamer GetGamerByUser(User user)
        {
            return _dbContext.Gamers.First(c=> c.GamerId == user.ID);
        }
        public async Task<Gamer> GetGamerById(string gamerId)
        {
            // Assuming GamerId is a string in MongoDB and comparing it with the given gamerId
            return await _mongoDbContext.Gamers
                .Find(c => c.GamerId == gamerId)
                .FirstOrDefaultAsync(); // Using MongoDB's async method
        }


    }
}

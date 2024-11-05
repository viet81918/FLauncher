using FLauncher.Model;
using FLauncher.Services;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FLauncher.DAO
{
    public class GamerDAO : SingletonBase<GamerDAO>
    {
        private readonly FlauncherDbContext _dbContext;

        public GamerDAO()
        {
            var connectionString = "mongodb://localhost:27017/";
            var client = new MongoClient(connectionString);
            _dbContext = FlauncherDbContext.Create(client.GetDatabase("FPT"));
        }
        public Gamer GetGamerByUser(User user)
        {
            return _dbContext.Gamers.First(c=> c.GamerId == user.ID);
        }
    }
}

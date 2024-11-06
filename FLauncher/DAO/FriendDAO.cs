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
    public class FriendDAO : SingletonBase<FriendDAO>
    {
        private readonly FlauncherDbContext _dbContext;

        public FriendDAO()
        {
            var connectionString = "mongodb://localhost:27017/";
            var client = new MongoClient(connectionString);
            _dbContext = FlauncherDbContext.Create(client.GetDatabase("FPT"));
        }
        public List<Friend> GetFriendInvitationsforGamer(Gamer gamer)
        {
           
            return _dbContext.Friends
                .Where(friend => friend.AcceptId == gamer.GamerId)
                .ToList();
        }

    }
}

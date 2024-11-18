using FLauncher.Model;
using FLauncher.Services;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FLauncher.DAO
{
    public class NotiDAO : SingletonBase<NotiDAO>
    {
        private readonly FlauncherDbContext _dbContext;

        public NotiDAO()
        {
            var connectionString = "mongodb://localhost:27017/";
            var client = new MongoClient(connectionString);
            _dbContext = FlauncherDbContext.Create(client.GetDatabase("FPT"));
        }
        public async Task<IEnumerable<Notification>> GetNotiforGamer(Gamer gamer) {
            return await _dbContext.Notifications
                    .Where(notification => notification.UserId == gamer.GamerId)
                    .ToListAsync();

        }
        public async Task<IEnumerable<Notification>> GetUnreadNotiforGamer(Gamer gamer)
        {
            // Fetch notifications where `UserId` matches and `isRead` is false
            return await _dbContext.Notifications
                             .Where(notification => notification.UserId == gamer.GamerId && !notification.isRead)
                             .ToListAsync();
        }

    }
}

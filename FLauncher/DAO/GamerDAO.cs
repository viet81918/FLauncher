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
            return _dbContext.Gamers.First(c => c.GamerId == user.ID);
        }

        public List<Gamer> GetGamersByIds(List<string> gamerIds)
        {
            // Truy vấn các gamer có GamerId trong danh sách gamerIds
            return _dbContext.Gamers
                             .Where(g => gamerIds.Contains(g.GamerId))
                             .ToList();
        }

        public List<Game> GetGamesByGamer(Gamer gamer)
        {
            // Lấy danh sách các GameID mà người chơi đã mua từ bảng Bills
            var purchasedGameIds = _dbContext.Bills
                                             .Where(b => b.GamerId == gamer.GamerId)
                                             .Select(b => b.GameId)
                                             .ToList();

            // Lấy thông tin các game từ bảng Games dựa trên danh sách GameID đã mua
            var games = _dbContext.Games
                                  .Where(g => purchasedGameIds.Contains(g.GameID))
                                  .ToList();

            return games;
        }

        public List<Buy> GetBillsByGamerId(Gamer gamer)
        {
            return _dbContext.Bills
                             .Where(b => b.GameId == gamer.GamerId)
                             .ToList();
        }

    }
}

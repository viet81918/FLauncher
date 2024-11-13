using FLauncher.Model;
using FLauncher.Services;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace FLauncher.DAO
{
    public class GameDAO : SingletonBase<GameDAO>
    {
        private readonly FlauncherDbContext _dbContext;

        public GameDAO()
        {
            var connectionString = "mongodb://localhost:27017/";
            var client = new MongoClient(connectionString);
            _dbContext = FlauncherDbContext.Create(client.GetDatabase("FPT"));
        }
        public List <Game> GetTopGames()
        {
            return  _dbContext.Games
                .OrderByDescending(g => g.NumberOfBuyers) // Sắp xếp giảm dần theo NumberOfBuyers
                .Take(9) // Lấy ra 9 game đầu tiên
                .ToList(); // Chuyển kết quả thành 
        }
            public List<Game> GetGamesByGamer(Gamer gamer)
        {
            // Lấy danh sách các GameID mà người chơi đã mua từ bảng Bills
            var purchasedGameIds = _dbContext.Bills
                                              .Where(b => b.GamerId == gamer.Id)
                                              .Select(b => b.GameId)
                                              .ToList();

            // Lấy thông tin các game từ bảng Games dựa trên danh sách GameID đã mua
            var games = _dbContext.Games
                                  .Where(g => purchasedGameIds.Contains(g.GameID))
                                  .ToList();

            return games;
        }
      


    }
}

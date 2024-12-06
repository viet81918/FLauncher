using FLauncher.Model;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using FLauncher.Services;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

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
        public async Task<IEnumerable<Gamer>> GetGamersFromGame(Game game)
        {
            // Retrieve all the Buy records that match the game's ID
            var buyRecords = await _dbContext.Bills
                .Where(b => b.GameId == game.GameID)  // Match Buy records with the given GameId
                .ToListAsync();

            // Retrieve the unique GamerIds from the Buy records
            var gamerIds = buyRecords.Select(b => b.GamerId).Distinct();

            // Query the Gamers collection for gamers who have bought the game
            var gamers = await _dbContext.Gamers
                .Where(g => gamerIds.Contains(g.GamerId))  // Match gamers by their GamerIds
                .ToListAsync();

            return gamers;
        }

        public Gamer GetGamerByUser(User user)
        {
            if (user?.ID == null)
            {
                throw new ArgumentNullException(nameof(user.ID), "User ID cannot be null.");
            }

            return _dbContext.Gamers.First(c => c.GamerId == user.ID);
        }


        public Gamer GetGamerById(string Id)
        {
            return _dbContext.Gamers.First(c => c.GamerId == Id);
        }


        public async Task<List<Gamer>> GetGamersByIds(List<string> gamerIds)
        {
            return await _dbContext.Gamers
                              .Where(g => gamerIds.Contains(g.GamerId))
                              .ToListAsync();
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

        public async Task<bool> IsUpdate(Game game, Gamer gamer)
        {
            if (game == null || gamer == null)
                return false;

            // Fetch the download record for the gamer and game
            var downloadRecord = await _dbContext.Downloads
                .FirstOrDefaultAsync(d => d.GameId == game.GameID && d.GamerId == gamer.GamerId);

            if (downloadRecord == null)
            {
                // No download record exists, so the game is not updated
                return false;
            }

            DateTime downloadTime = downloadRecord.TimeDownload;

            // Fetch the last update time for the game
            var updateRecord = await _dbContext.Updates
       .OrderByDescending(d => d.UpdateTimeString) // Use UpdateTimeString for ordering
       .FirstOrDefaultAsync(u => u.GameId == game.GameID);

            if (updateRecord == null)
            {
                // No updates have been made for this game
                return true;
            }

            DateTime lastUpdateTime = updateRecord.UpdateTime;

            // Check if the download time is after the last update time
            return downloadTime >= lastUpdateTime;
        }




    }
}

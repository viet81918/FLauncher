using FLauncher.Model;
using FLauncher.Services;
using FLauncher.ViewModel;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FLauncher.DAO
{
    class TrackingDAO : SingletonBase<TrackingDAO>
    {
        private readonly FLauncherOnLineDbContext _dbContext;
      
        public TrackingDAO()
        {

            var connectionString = "mongodb+srv://viet81918:conchode239@cluster0.hzr2fsy.mongodb.net/?retryWrites=true&w=majority&appName=Cluster0";
            var client1 = new MongoClient(connectionString);
            _dbContext = FLauncherOnLineDbContext.Create(client1.GetDatabase("FPT"));
           
        }

        public async Task<IEnumerable<TrackingRecords>> GetTrackingFromGamerGame(Gamer gamer, Game game)
        {
            // Ensure the Day field in MongoDB is formatted as "dd/MM/yyyy"
            string todayDateString = DateTime.Today.ToString("dd/MM/yyyy");

            var collection = await _dbContext.TrackingsTime
                .Where(c => c.ID_Gamer == gamer.GamerId
                            && c.ID_Game == game.GameID
                            && c.DateString == todayDateString) // Query directly on DateString
                .ToListAsync();

            //foreach (var tracking in collection)
            //{
            //    MessageBox.Show(tracking.DateString);  // Use DateString for displaying the date
            //}

            return collection;
        }
        private async Task<string> GetGameNameAsync(string gameId)
        {
            var game = await _dbContext.Games.FirstOrDefaultAsync(g => g.GameID == gameId);
            return game?.Name ?? "Unknown Game";
        }

        public async Task<IEnumerable<(Game Game, double TotalHours, DateTime LastPlayed)>> GetGamesWithPlayingHoursAndLastPlayedAsync(string gamerId)
        {
            // Fetch tracking records for the given gamer
            var trackingRecords = await _dbContext.TrackingsTime
                .Where(t => t.ID_Gamer == gamerId && t.TimePlayed > 0) // Ensure valid playtime
                .ToListAsync();

            if (!trackingRecords.Any())
            {
                return Enumerable.Empty<(Game, double, DateTime)>();
            }

            // Collect distinct game IDs from tracking records
            var gameIds = trackingRecords
                .Select(t => t.ID_Game)
                .Distinct();

            // Fetch all corresponding Game objects in one query
            var games = await _dbContext.Games
                .Where(g => gameIds.Contains(g.GameID))
                .ToDictionaryAsync(g => g.GameID);

            // Group tracking records by game ID and calculate statistics
            var results = trackingRecords
                .GroupBy(t => t.ID_Game)
                .Select(group =>
                {
                    // Try to get the Game object from the dictionary
                    if (!games.TryGetValue(group.Key, out var game))
                    {
                        return default; // Skip if game is not found
                    }

                    // Calculate total hours and last played date
                    var totalHours = Math.Round(group.Sum(t => t.TimePlayed) / 3600.0,1); // Convert seconds to hours
                    var lastPlayed = group.Max(t => t.Date);

                    return (Game: game, TotalHours: totalHours, LastPlayed: lastPlayed);
                })
                .Where(result => result.Game != null) // Filter out null results
                .ToList();

            return results;
        }





        public async Task<TrackingPlayers> GetTrackingFromGame(Game game)
        {
        
            var collection = await _dbContext.TrackingPlayers
                .Where(c => c.ID_Game == game.GameID).AsNoTracking()
                .FirstOrDefaultAsync();

            return collection;
        }
        



    }
}

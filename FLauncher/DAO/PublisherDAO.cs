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
    public class PublisherDAO : SingletonBase<PublisherDAO>
    {
        private readonly FlauncherDbContext _dbContext;

        public PublisherDAO()
        {
            var connectionString = "mongodb://localhost:27017/";
            var client = new MongoClient(connectionString);
            _dbContext = FlauncherDbContext.Create(client.GetDatabase("FPT"));
        }
        public async Task<GamePublisher> GetPublisherByGame(Game game)
        {
            // Step 1: Get the corresponding 'Publish' entry for the game
            var publishEntry = _dbContext.Publishcations
                                        .FirstOrDefault(p => p.GameId == game.GameID && p.isPublishable);

            if (publishEntry == null)
            {
                // If no publisher entry is found, return null or handle accordingly
                return null;
            }

            // Step 2: Get the 'GamePublisher' based on the GamePublisherId in the publishEntry
            var publisher = await _dbContext.GamePublishers
                                      .FirstOrDefaultAsync(p => p.PublisherId == publishEntry.GamePublisherId);

            // Return the publisher object
            return publisher;
        }
        public async Task<IEnumerable<Update>> getUpdatesForGame(Game game)
        {
            return await _dbContext.Updates.Where(c => c.GameId == game.GameID).ToListAsync();
        }
        public GamePublisher GetPublisherByUser(User user)
        {
            return _dbContext.GamePublishers.First(c => c.PublisherId == user.ID);
        }
    }
}

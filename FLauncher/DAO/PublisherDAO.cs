using FLauncher.Model;
using FLauncher.Services;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
        public List<string> getAll()
        {
            return _dbContext.GamePublishers
                           .Select(p => p.Name)  // Chọn chỉ trường TypeOfGenre
                           .ToList();
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

        public async Task<IEnumerable<GamePublisher>> GetTopPublishersAsync()
        {
            // Step 1: Get all publication data from the database (to map Game to PublisherId)
            var publicationData = await _dbContext.Publishcations
                .Select(publication => new
                {
                    publication.GameId,         // GameId from Publication
                    publication.GamePublisherId // PublisherId from Publication
                })
                .ToListAsync();

            // Step 2: Get all game data, including the GameId and NumberOfBuyers
            var gameData = await _dbContext.Games
                .Select(game => new
                {
                    game.GameID,                 // GameId from Game
                    game.NumberOfBuyers         // Number of buyers for the game
                })
                .ToListAsync();

            // Step 3: Get all publisher data (entire GamePublisher object)
            var publisherData = await _dbContext.GamePublishers
                .Where(publisher => publicationData.Select(pub => pub.GamePublisherId).Contains(publisher.PublisherId))  // Filter publishers based on the PublisherId in publications
                .ToListAsync(); // Fetch all publisher data as a list of GamePublisher objects

            // Step 4: Aggregate total buyers by PublisherId
            var publisherBuyerCounts = publisherData
                .Select(publisher =>
                {
                    // Get the games related to the current publisher via publication data
                    var publisherGames = publicationData
                        .Where(pub => pub.GamePublisherId == publisher.PublisherId)
                        .Select(pub => pub.GameId)
                        .ToList();

                    // Sum the total number of buyers for all games by the current publisher
                    var totalBuyers = gameData
                        .Where(game => publisherGames.Contains(game.GameID))  // Only sum buyers for games from the publisher's games
                        .Sum(game => game.NumberOfBuyers);

                    // Return the full GamePublisher object with the total buyers
                    return new { Publisher = publisher, TotalBuyers = totalBuyers };
                })
                .OrderByDescending(x => x.TotalBuyers)  // Order by total buyers in descending order
                .Take(9)  // If you only want top 9 publishers
                .ToList(); // Materialize the result into a list

            // Step 5: Return only the full GamePublisher objects
            return publisherBuyerCounts.Select(x => x.Publisher);
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

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
    public class GenreDAO : SingletonBase<GenreDAO>
    {
        private readonly FlauncherDbContext _dbContext;

        public GenreDAO()
        {
            var connectionString = "mongodb://localhost:27017/";
            var client = new MongoClient(connectionString);
            _dbContext = FlauncherDbContext.Create(client.GetDatabase("FPT"));
        }
        public List<Genre> GetGenres()
        {
            return _dbContext.Genres.ToList();
        }
        public List<Genre> GetGenresFromGame(Game game)
        {
            // Step 1: Get all related Genre names from Game_Has_Genre
            var genreNames = _dbContext.GameHasGenres
                .Where(c => c.GameId == game.GameID)
                .Select(c => c.TypeOfGenres) // Get Genre names from Game_Has_Genre
                .ToList();

            // Step 2: Use the retrieved genre names to get Genre details from the Genres collection
            var genres = _dbContext.Genres
                .Where(g => genreNames.Contains(g.TypeOfGenre)) // Use Contains to filter by multiple genres
                .ToList();

            return genres;
        }


    }
}

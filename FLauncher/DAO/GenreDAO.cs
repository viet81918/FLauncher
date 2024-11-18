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
    public class GenreDAO : SingletonBase<GenreDAO>
    {
        private readonly FlauncherDbContext _dbContext;

        public GenreDAO()
        {
            var connectionString = "mongodb://localhost:27017/";
            var client = new MongoClient(connectionString);
            _dbContext = FlauncherDbContext.Create(client.GetDatabase("FPT"));
        }
        public async Task<IEnumerable<Genre>> GetGenres()
        {
            return await _dbContext.Genres.ToListAsync();
        }
        public async Task<IEnumerable<Genre>> GetGenresFromGame(Game game)
        {
            // Step 1: Get all related Genre names from Game_Has_Genre
            var genreNames = _dbContext.GameHasGenres
                .Where(c => c.GameId == game.GameID)
                .Select(c => c.TypeOfGenres) // Get Genre names from Game_Has_Genre
                .ToList();

            // Step 2: Use the retrieved genre names to get Genre details from the Genres collection
            var genres = await _dbContext.Genres
                .Where(g => genreNames.Contains(g.TypeOfGenre)) // Use Contains to filter by multiple genres
                .ToListAsync();

            return genres;
        }


    }
}

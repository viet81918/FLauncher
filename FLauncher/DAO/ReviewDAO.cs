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
    public class ReviewDAO : SingletonBase<ReviewDAO>
    {
        private readonly FlauncherDbContext _dbContext;

        public ReviewDAO()
        {
            var connectionString = "mongodb://localhost:27017/";
            var client = new MongoClient(connectionString);
            _dbContext = FlauncherDbContext.Create(client.GetDatabase("FPT"));
        }
        public async Task<IEnumerable<Review>> GetReviewsByGame (Game game)
        {
            return await _dbContext.Reviews.Where(c => c.GameId == game.GameID)
                .ToListAsync(); 
        }
    
    }
}

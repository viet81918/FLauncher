using FLauncher.Model;
using FLauncher.Services;
using Microsoft.EntityFrameworkCore;
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
        private readonly FLauncherOnLineDbContext _dbContextOnline;
        public TrackingDAO()
        {

            var connectionString = "mongodb+srv://viet81918:conchode239@cluster0.hzr2fsy.mongodb.net/?retryWrites=true&w=majority&appName=Cluster0";
            var client = new MongoClient(connectionString);
            _dbContextOnline = FLauncherOnLineDbContext.Create(client.GetDatabase("FPT"));
        }

        public async Task<IEnumerable<TrackingRecords>> GetTrackingFromGamerGame(Gamer gamer, Game game)
        {
            // Ensure the Day field in MongoDB is formatted as "dd/MM/yyyy"
            string todayDateString = DateTime.Today.ToString("dd/MM/yyyy");

            var collection = await _dbContextOnline.TrackingsTime
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
        public async Task<TrackingPlayers> GetTrackingFromGame( Game game)
        {


            var collection = await _dbContextOnline.TrackingPlayers
                .FirstOrDefaultAsync(c => c.ID_Game == game.GameID)
                    ;
           

            return collection;
        }




    }
}

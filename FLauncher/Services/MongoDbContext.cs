using FLauncher.Model;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FLauncher.Services
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext()
        {
            var client = new MongoClient("mongodb://localhost:27017/");
            _database = client.GetDatabase("FPT");
        }

        public IMongoCollection<Achivement> Achivements => _database.GetCollection<Achivement>("Achivements");
        public IMongoCollection<Admin> Admins => _database.GetCollection<Admin>("Admins");
        public IMongoCollection<Buy> Bills => _database.GetCollection<Buy>("Bills");
        public IMongoCollection<Category> Categories => _database.GetCollection<Category>("Categories");
        public IMongoCollection<Download> Downloads => _database.GetCollection<Download>("Downloads");
        public IMongoCollection<Friend> Friends => _database.GetCollection<Friend>("Friends");
        public IMongoCollection<Game> Games => _database.GetCollection<Game>("Games");
        public IMongoCollection<Game_Has_Genre> GameHasGenres => _database.GetCollection<Game_Has_Genre>("GameHasGenres");
        public IMongoCollection<GamePublisher> GamePublishers => _database.GetCollection<GamePublisher>("GamePublishers");
        public IMongoCollection<Message> Messages => _database.GetCollection<Message>("Messages");
        public IMongoCollection<Notification> Notifications => _database.GetCollection<Notification>("Notifications");
        public IMongoCollection<Publish> Publishcations => _database.GetCollection<Publish>("Publishcations");
        public IMongoCollection<Review> Reviews => _database.GetCollection<Review>("Reviews");
        public IMongoCollection<UnlockAchivement> UnlockAchivements => _database.GetCollection<UnlockAchivement>("UnlockAchivements");
        public IMongoCollection<Genre> Genres => _database.GetCollection<Genre>("Genres");
        public IMongoCollection<Update> Updates => _database.GetCollection<Update>("Updates");
        public IMongoCollection<User> Users => _database.GetCollection<User>("Users");
        public IMongoCollection<Gamer> Gamers => _database.GetCollection<Gamer>("Gamers");
    }
}

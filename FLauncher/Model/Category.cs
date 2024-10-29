using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.EntityFrameworkCore;
namespace FLauncher.Model
{
    [Collection("Category")]
    public class Category
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("Gamer_id")] // Matches the JSON key "Gamer_id"
        public string GamerId { get; set; }

        [BsonElement("NameCategories")] // Matches the JSON key "NameCategories"
        public string NameCategories { get; set; }

        [BsonElement("GameList")] // Matches the JSON key for the list of games
        public virtual ICollection<Game> GameList { get; set; } = new List<Game>(); // Initializes the list
    
}
}

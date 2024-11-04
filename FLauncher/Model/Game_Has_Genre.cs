using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.EntityFrameworkCore;

namespace FLauncher.Model
{
    [Collection("Game_Has_Genre")]
    public class Game_Has_Genre
    {
        [BsonId] // Marks this as the primary key in MongoDB
        [BsonRepresentation(BsonType.ObjectId)] // Maps MongoDB ObjectId to a C# string
        public string Id { get; set; }

        [BsonElement("ID_Game")] // Maps to "ID_Game" in MongoDB
        public string GameId { get; set; }

        [BsonElement("Type_of_genres")] // Maps to "Type_of_genres" in MongoDB
        public string TypeOfGenres { get; set; }
    }
}

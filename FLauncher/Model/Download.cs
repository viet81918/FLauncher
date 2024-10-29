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
    [Collection("Download")]
    public class Download
    {
        [BsonId] // Marks this as the primary key in MongoDB
        [BsonRepresentation(BsonType.ObjectId)] // Maps MongoDB ObjectId to a C# string
        public string Id { get; set; }

        [BsonElement("Gamer_id")] // Matches the JSON key "Gamer_id"
        public string GamerId { get; set; }

    [BsonElement("Game_id")] // Matches the JSON key "Game_id"
    public string GameId { get; set; }

    [BsonElement("TimeDownload")] // Matches the JSON key "TimeDownload"
    public string TimeDownload { get; set; }

    [BsonElement("Storage")] // Matches the JSON key "Storage"
    public string Storage { get; set; }

    [BsonElement("Directory")] // Matches the JSON key "Directory"
    public string Directory { get; set; }

    [BsonElement("isDownload")] // Matches the JSON key "isDownload"
    public bool IsDownload { get; set; }
    }
}

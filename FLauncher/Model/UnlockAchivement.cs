using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using MongoDB.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FLauncher.Model
{
    [Collection("UnlockAchivement")]
    public class UnlockAchivement
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }  // Optional: Add this if you want a unique identifier for each record

        [BsonElement("Gamer_id")]
        public string GamerId { get; set; }

        [BsonElement("Game_id")]
        public string GameId { get; set; }

        [BsonElement("Achivement_id")]
        public string AchievementId { get; set; }

        [BsonElement("DateUnlock")]
        public DateTime DateUnlock { get; set; }
    }
}

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
    [Collection("Achivement")]
    public class Achivement 
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)] // Automatically maps to MongoDB ObjectId
        public string Id { get; set; }
        [BsonElement("Achivement_Id")]
        public string AchivementId { get; set; }
        [BsonElement("Name")]
        public string Name { get; set; }
        [BsonElement("Game_Id")]
        public string GameId { get; set; }
        [BsonElement("Trigger")]
        public string Trigger { get; set; }
        [BsonElement("UnlockImageLink")]
        public string UnlockImageLink { get; set; }
        [BsonElement("LockImageLink")]
        public string LockImageLink { get; set; }

        [BsonElement("IsPrivate")]
        public bool IsPrivate { get; set; } = false;
        
    }
}

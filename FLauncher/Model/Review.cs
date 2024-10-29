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
    [Collection("Reviews")]
    public class Review
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("ID_Gamer")]
        public string GamerId { get; set; }

        [BsonElement("ID_Game")]
        public string GameId { get; set; }

        public int Rating { get; set; }

        public string Description { get; set; }
  
    }
}

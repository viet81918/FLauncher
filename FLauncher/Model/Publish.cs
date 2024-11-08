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
    [Collection("Publish")]
    public class Publish
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("ID_Game")]
        public string GameId { get; set; }

        [BsonElement("ID_Game_Publisher")]
        public string GamePublisherId { get; set; }

        [BsonElement("ID_Admin")]
        public string AdminId { get; set; }

        public Boolean isPublishable { get; set; }
    }
}

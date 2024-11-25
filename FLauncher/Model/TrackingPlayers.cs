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
    [Collection("TrackingPlayer")]
    public class TrackingPlayers
    {

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [BsonElement("ID_Game")]

        public string ID_Game { get; set; }
        [BsonElement("CurrentPlayer")]

        public int CurrentPlayer { get; set; }
        [BsonElement("PlayerList")]
        public string[] PlayerIds { get; set; }
    }
}

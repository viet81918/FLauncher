using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using MongoDB.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

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

        [BsonElement("PlayerChange")]
        public int[] PlayerChange { get; set; } // Stores the sequence of player changes.

        [BsonElement("TimePlayerChange")]
        public string[] TimePlayerChangeStrings { get; set; } // Maps to the string array in the document.

        [BsonIgnore]
        public DateTime[] TimePlayerChange
        {
            get
            {
                return TimePlayerChangeStrings?
                    .Select(t => DateTime.ParseExact(t, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture))
                    .ToArray();
            }
        }
    }


}

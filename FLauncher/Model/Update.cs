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
    [Collection("Update")]
    public  class Update
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }  // Optional: Add this if you want a unique identifier for each update

        [BsonElement("Publisher_id")]
        public string PublisherId { get; set; }

        [BsonElement("Game_id")]
        public string GameId { get; set; }

        [BsonElement("UpdateContent")]
        public string UpdateContent { get; set; }

        [BsonElement("UpdateTime")]
        public string UpdateTimeString { get; set; }

        [BsonIgnoreIfNull]
        // Converts UpdateTime to DateTime when accessed
        [BsonIgnore]
        public DateTime UpdateTime
        {
            get => DateTime.Parse(UpdateTimeString); // Converts string to DateTime
            set => UpdateTimeString = value.ToString("yyyy-MM-dd HH:mm:ss"); // Formats DateTime as string
        }

      
    }
}

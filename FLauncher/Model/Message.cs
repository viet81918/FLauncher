using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FLauncher.Model
{
    [Collection("Message")]
    public  class Message
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        
        [BsonElement("id_sender")]
        public string IdSender { get; set; }

        [BsonElement("id_receiver")]
        public string IdReceiver { get; set; }

        public string Content { get; set; }

        [BsonElement("Time")]
        public string TimeString { get; set; } // Stores as string from MongoDB

        [BsonIgnore]
        public DateTime Time
        {
            get => DateTime.Parse(TimeString); // Converts the string to DateTime when accessed
            set => TimeString = value.ToString("yyyy-MM-dd HH:mm:ss"); // Converts DateTime to string
        }
    }
}

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
    [Collection("Notification")]
    public class Notification
    {

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("User_id")]
        public string UserId { get; set; }

        [BsonElement("TimeNotification")]
        public string TimeNotificationString { get; set; }

        // Converts TimeNotification to DateTime when accessed
        [BsonIgnore]
        public DateTime TimeNotification
        {
            get => DateTime.Parse(TimeNotificationString); // Converts string to DateTime
            set => TimeNotificationString = value.ToString("yyyy-MM-dd HH:mm:ss"); // Formats DateTime as string
        }


        public string Content { get; set; }
    }
}

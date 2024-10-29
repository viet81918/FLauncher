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

        public string TimeNotification { get; set; }  // Store as string or convert to DateTime if needed

        public string Content { get; set; }
    }
}

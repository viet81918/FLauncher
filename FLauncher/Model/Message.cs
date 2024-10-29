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

        public DateTime Time { get; set; }
    }
}

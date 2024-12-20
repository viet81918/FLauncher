﻿using MongoDB.Bson;
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
    public class Message
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        
        [BsonElement("id_sender")]
        public string IdSender { get; set; }

        [BsonElement("id_receiver")]
        public string IdReceiver { get; set; }

        [BsonElement("Content")]
        public string Content { get; set; }

        [BsonElement("Time")]
        public DateTime TimeString { get; set; } // Stores as string from MongoDB      
        [BsonIgnore]
        public bool IsSenderCurrentUser { get; set; }
    }
}

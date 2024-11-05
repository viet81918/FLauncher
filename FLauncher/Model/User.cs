using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.EntityFrameworkCore;

namespace FLauncher.Model
{
    [Collection("Users")]
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("ID")]
        public string ID { get; set; }

        public string Name { get; set; }

        public string Password { get; set; }

        [BsonElement("Email")]
        public string Email { get; set; }
        public int Role { get; set; }
    }
}

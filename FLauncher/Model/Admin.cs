using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.EntityFrameworkCore;
namespace FLauncher.Model
{

    [Collection("Admins")]
    public class Admin
    {

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)] // Automatically maps to MongoDB ObjectId
        public string Id { get; set; }

        [BsonElement("ID")] // Matches the JSON field name "ID"
        public string UserID { get; set; }
        [BsonElement("Gmail")] // Matches the JSON field name "Gmail"
        public string Gmail { get; set; }
        [BsonElement("Name")] // Matches the JSON field name "Name"
        public string Name { get; set; }
        [BsonElement("Password")] // Matches the JSON field name "Password"
        public string Password { get; set; }
        [BsonElement("Role")] // Matches the JSON field name "Role"
        public int Role { get; set; }
    
}
}

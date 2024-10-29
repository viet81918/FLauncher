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
    [Collection("Gamers")]
    public class Gamer
    {
        [BsonId] // Marks this as the primary key in MongoDB
        [BsonRepresentation(BsonType.ObjectId)] // Maps MongoDB ObjectId to a C# string
        public string Id { get; set; }

        [BsonElement("ID")] // Maps to "ID" in MongoDB
        public string GamerId { get; set; }

        [BsonElement("Name")] // Maps to "Name" in MongoDB
        public string Name { get; set; }

        [BsonElement("Email")] // Maps to "Email" in MongoDB
        public string Email { get; set; }

        [BsonElement("Password")] // Maps to "Password" in MongoDB
        public string Password { get; set; }

        [BsonElement("Money")] // Maps to "Money" in MongoDB
        public double Money { get; set; }

        [BsonElement("AvatarLink")] // Maps to "AvatarLink" in MongoDB
        public string AvatarLink { get; set; }

        [BsonElement("Role")] // Maps to "Role" in MongoDB
        public int Role { get; set; }

        [BsonElement("RegistrationDate")] // Maps to "RegistrationDate" in MongoDB
        public string RegistrationDate { get; set; }

        [BsonElement("Date of Birth")] // Maps to "Date of Birth" in MongoDB
        public DateTime DateOfBirth { get; set; }
    }
}

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
    [Collection("GamePublishers")]
    public class GamePublisher
    {
        [BsonId] // Marks this as the primary key in MongoDB
        [BsonRepresentation(BsonType.ObjectId)] // Maps MongoDB ObjectId to a C# string
        public string Id { get; set; }

        [BsonElement("ID")] // Maps to "ID" in MongoDB
        public string PublisherId { get; set; }

        [BsonElement("Name")] // Maps to "Name" in MongoDB
        public string Name { get; set; }

        [BsonElement("Password")] // Maps to "Password" in MongoDB
        public string Password { get; set; }

        [BsonElement("Email")] // Maps to "Email" in MongoDB
        public string Email { get; set; }

        [BsonElement("Bank_account")] // Maps to "Bank_account" in MongoDB
        public string BankAccount { get; set; }

        [BsonElement("Profit")] // Maps to "Profit" in MongoDB
        public double Profit { get; set; }

        [BsonElement("Description")] // Maps to "Description" in MongoDB
        public string Description { get; set; }

        [BsonElement("AvatarLink")] // Maps to "AvatarLink" in MongoDB
        public string AvatarLink { get; set; }

        [BsonElement("Money")] // Maps to "Money" in MongoDB
        public double Money { get; set; }

        [BsonElement("Role")] // Maps to "Role" in MongoDB
        public int Role { get; set; }
        [BsonElement("RegistrationDate")]
        public string RegistrationDateString { get; set; } // Store as string in MongoDB

        [BsonIgnore]
        public DateTime RegistrationDate
        {
            get => DateTime.ParseExact(RegistrationDateString, "dd/MM/yyyy", null); // Converts string to DateTime
            set => RegistrationDateString = value.ToString("dd/MM/yyyy"); // Converts DateTime to string
        }
    }
}

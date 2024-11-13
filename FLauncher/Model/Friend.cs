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
    [Collection("Friends")]
    public class Friend
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)] // Maps MongoDB ObjectId to a C# string
        public string Id { get; set; }

        [BsonElement("Request_id")]
        public string RequestId { get; set; }  // Represents the ID of the player who sent the invite

        [BsonElement("Accept_id")]
        public string AcceptId { get; set; }  // Represents the ID of the player who is supposed to accept the invite

        [BsonElement("InvitationTime")]
        public string InvitationTimeString { get; set; }  // Stored as a string in MongoDB

        [BsonIgnore]
        public DateTime InvitationTime
        {
            get => DateTime.ParseExact(InvitationTimeString, "yyyy-MM-dd HH:mm:ss", null);  // Converts string to DateTime
            set => InvitationTimeString = value.ToString("yyyy-MM-dd HH:mm:ss");  // Converts DateTime to string
        }

        [BsonElement("isAccept")]
        public bool? IsAccept { get; set; }  // Nullable bool to store whether the invitation is accepted
    }

}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.EntityFrameworkCore;
namespace FLauncher.Model
{
    [Collection("Friend")]
    public  class Friend
    {
        [BsonId] // Marks this as the primary key in MongoDB
        [BsonRepresentation(BsonType.ObjectId)] // Maps MongoDB ObjectId to a C# string
        public string Id { get; set; }

        [BsonElement("Request_id")] // Maps to "Request_id" in MongoDB
        public string RequestId { get; set; }

        [BsonElement("Accept_id")] // Maps to "Accept_id" in MongoDB
        public string AcceptId { get; set; }

        [BsonElement("InvitationTime")] // Maps to "InvitationTime" in MongoDB
        public DateTime InvitationTime { get; set; }

        [BsonElement("isAccept")] // Maps to "isAccept" in MongoDB, nullable bool
        public bool? IsAccept { get; set; }
    }
}
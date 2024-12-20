﻿using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using MongoDB.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FLauncher.Model
{
    [Collection("UnlockAchivement")]
    public class UnlockAchivement
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }  // Optional: Add this if you want a unique identifier for each record

        [BsonElement("ID_Gamer")]
        public string GamerId { get; set; }

        [BsonElement("ID_Game")]
        public string GameId { get; set; }

        [BsonElement("Achivement_id")]
        public string AchievementId { get; set; }

        [BsonElement("DateUnlock")]
        public string DateUnlockString { get; set; }

        // Converts DateUnlock to DateTime when accessed
        [BsonIgnore]
        public DateTime DateUnlock
        {
            get => DateTime.Parse(DateUnlockString); // Converts string to DateTime
            set => DateUnlockString = value.ToString("yyyy-MM-dd HH:mm:ss"); // Formats DateTime as string
        }
    }
}

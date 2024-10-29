﻿using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.EntityFrameworkCore;

namespace FLauncher.Model
{
    [Collection("Games")]
    public  class Game
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("ID")]
        public string GameID { get; set; }

        public string Name { get; set; }

        public int Price { get; set; }

        [BsonElement("Publish_day")]
        public string PublishDay { get; set; }  // Store as string or DateTime if parsing is required

        [BsonElement("Number_of_buyers")]
        public int NumberOfBuyers { get; set; }

        public string LinkTrailer { get; set; }

        public string AvatarLink { get; set; }

        public string GameLink { get; set; }

        public string Description { get; set; }

        [BsonElement("Minimum_CPU")]
        public string MinimumCPU { get; set; }

        [BsonElement("Minimum_RAM")]
        public string MinimumRAM { get; set; }

        [BsonElement("Minimum_GPU")]
        public string MinimumGPU { get; set; }

        [BsonElement("Maximum_CPU")]
        public string MaximumCPU { get; set; }

        [BsonElement("Maximum_RAM")]
        public string MaximumRAM { get; set; }

        [BsonElement("Maximum_GPU")]
        public string MaximumGPU { get; set; }
    }
}
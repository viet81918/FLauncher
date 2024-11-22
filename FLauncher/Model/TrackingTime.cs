using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FLauncher.Model
{
        public class TrackingTime
        {
       
            [JsonProperty("ID")]
            public string ID { get; set; }

         
            [JsonProperty("ID_Game")]
            public string ID_Game { get; set; }

            [JsonProperty("ID_Gamer")]
            public string ID_Gamer { get; set; }

            [JsonProperty("TimePlayed")]
            public int TimePlayed { get; set; }

            [JsonProperty("TimeStart")]
            public string TimeStart { get; set; }

            [JsonProperty("TimeEnd")]
            public string TimeEnd { get; set; }

            [JsonProperty("Date")]
            [Newtonsoft.Json.JsonConverter(typeof(CustomDateTimeConverter))]  // Apply custom date converter here
     
            public DateTime Date { get; set; }

            [JsonProperty("IsPlayed")]
            public int IsPlayed { get; set; }
        }

}

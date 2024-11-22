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
    [Collection("TrackingTime")]
    public class TrackingRecords
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("ID")]
        public string ID { get; set; }

        [BsonElement("ID_Game")]
     
        public string ID_Game { get; set; }

        [BsonElement("ID_Gamer")]
      
        public string ID_Gamer { get; set; }

        [BsonElement("TimePlay")]
  
        public int TimePlayed { get; set; }

        [BsonElement("Time_start")]
      
        public string TimeStart { get; set; }

        [BsonElement("Time_end")]
      
        public string TimeEnd { get; set; }
        [BsonElement("Day")]
        public string DateString { get; set; }
        [BsonIgnore]
        public DateTime Date
        {
            get => DateTime.ParseExact(DateString, "dd/MM/yyyy", null);
            set => DateString = value.ToString("dd/MM/yyyy");
        }

        [BsonElement("IsPlayed")]
      
        public int IsPlayed { get; set; }
    }

}

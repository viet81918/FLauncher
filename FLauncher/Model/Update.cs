using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using MongoDB.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace FLauncher.Model
{
    [Collection("Update")]
    public class Update
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("Publisher_id")]
        public string PublisherId { get; set; }

        [BsonElement("Game_id")]
        public string GameId { get; set; }

        [BsonElement("UpdateContent")]
        public string UpdateContent { get; set; }

        [BsonElement("UpdateTime")]
        public string UpdateTimeString { get; set; }


        [BsonIgnore]
        public DateTime UpdateTime
        {
            get
            {
              

                if (DateTime.TryParseExact(UpdateTimeString, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result))
                {
                    return result;
                }

                throw new InvalidOperationException($"Invalid UpdateTime format: {UpdateTimeString}");
            }
            set
            {
                UpdateTimeString = value.ToString("yyyy-MM-dd HH:mm:ss");
            }
        }

    }


}

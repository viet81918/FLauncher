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
    [Collection("Achivement")]
    public class Achivement
    {
        public ObjectId Id { get; set; }

        public string AchivementId { get; set; }
        public string Name { get; set; }

        public string GameId { get; set; }

        public string Trigger { get; set; }

        public string UnlockImageLink { get; set; }

        public string LockImageLink { get; set; }

        [BsonIgnoreIfNull] // Sets this property to be nullable in MongoDB
        public bool? IsPrivate { get; set; }
    }
}

using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.EntityFrameworkCore;

namespace FLauncher.Model
{
    [Collection("Genres")]
    public  class Genre
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("Type_of_Genre")]
        public string TypeOfGenre { get; set; }

        public string Description { get; set; }
    }
}

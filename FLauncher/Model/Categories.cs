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
    [Collection("Categories")]
    public class Categories
    {
        public ObjectId Id { get; set; }

   
        public string NameCategory { get; set; }

   
        public string GameId { get; set; }
    }
}

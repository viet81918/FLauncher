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
    [Collection("Category")]
    public class Category
    {
        public ObjectId Id { get; set; }

 
        public string GamerId { get; set; }

        public string NameCategories { get; set; }
    }
}

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

    [Collection("Admins")]
    public class Admins
    {
       
        public ObjectId Id { get; set; }

      
        public string AdminId { get; set; } // Use a different property name in C# if desired

        public string Gmail { get; set; }

        public string Name { get; set; }

        public string Password { get; set; }

        public int Role { get; set; }
    }
}

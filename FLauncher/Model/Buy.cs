using MongoDB.Bson.Serialization.Attributes;
using MongoDB.EntityFrameworkCore;
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
    [Collection("Buy")]
    public class Buy
    {
        public ObjectId Id { get; set; }

        public string BillId { get; set; }
        public string GamerId { get; set; }

        public string GameId { get; set; }

        public DateTime BuyTime { get; set; }

        public decimal BuyPrice { get; set; }
    }
}

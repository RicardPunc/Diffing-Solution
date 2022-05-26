using Diffing_API.IContext;
using Diffing_API.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Diffing_API.Context
{
    public class DiffContext : IDiffContext
    {
        public DiffContext()
        {
            var client = new MongoClient("mongodb://mongodbData:27017");
            var database = client.GetDatabase("DiffDataDb");
            DiffData = database.GetCollection<DiffModel>("DiffData");
        }
        public IMongoCollection<DiffModel> DiffData { get; }
    }
}

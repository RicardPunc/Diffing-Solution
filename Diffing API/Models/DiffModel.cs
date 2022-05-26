using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Diffing_API.Models
{
    public class DiffModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.Int32)]
        public int ID { get; set; }

        public RequestData left { get; set; }

        public RequestData right { get; set; }

        public DiffModel(int ID)
        {
            this.ID = ID;
            this.left = null;
            this.right = null;
        }
    }
}

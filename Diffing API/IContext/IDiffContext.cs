using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Diffing_API.Models;
using MongoDB.Driver;

namespace Diffing_API.IContext
{
    public interface IDiffContext
    {
        IMongoCollection<DiffModel> DiffData { get; }
    }
}

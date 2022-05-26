using Diffing_API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Diffing_API.IRepository
{
    public interface IDiffRepository
    {
        Task<DiffModel> GetData(int diffID);
        Task PostData(DiffModel diff);
    }
}

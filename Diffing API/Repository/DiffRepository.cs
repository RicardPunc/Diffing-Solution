using Diffing_API.IContext;
using Diffing_API.IRepository;
using Diffing_API.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Diffing_API.Repository
{
    public class DiffRepository : IDiffRepository
    {
        private readonly IDiffContext _context;
        

        public DiffRepository(IDiffContext context)
        {
            _context = context;
        }

        public async Task PostData(DiffModel diff)
        {
            FilterDefinition<DiffModel> filter = Builders<DiffModel>.Filter.Eq(p => p.ID, diff.ID);

            DiffModel model = await _context.DiffData.FindOneAndReplaceAsync(filter, diff);

            if (model == null)
            {
                await _context.DiffData.InsertOneAsync(diff);
            }
            
        }
        public async Task<DiffModel> GetData(int diffID)
        {
            FilterDefinition<DiffModel> filter = Builders<DiffModel>.Filter.Eq(p => p.ID, diffID);
            return await _context
                                        .DiffData
                                        .Find(filter)
                                        .FirstOrDefaultAsync();
        }

    }
}


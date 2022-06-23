using Microsoft.EntityFrameworkCore;
using OkxPerpetualArbitrage.Application.Contracts.Persistance;
using OkxPerpetualArbitrage.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OkxPerpetualArbitrage.Persistance.Repositories
{
    class PotentialPositionRepository : AsyncRepository<PotentialPosition>, IPotentialPositionRepository
    {
        private readonly object updateLock = new object();
        public PotentialPositionRepository(SpotPerpDbContext spotPerpDbContext) : base(spotPerpDbContext)
        {
        }

        public async Task<PotentialPosition> GetPotentialPosition(string symbol)
        {
            return await _dbContext.PotentialPositions.Where(x => x.Symbol == symbol).FirstOrDefaultAsync();
        }

        public async Task UnUpToDate(string symbol)
        {
            var pp = await _dbContext.PotentialPositions.Where(x => x.Symbol == symbol).FirstOrDefaultAsync();
            if (pp != null)
            {
                pp.IsUpToDate = false;
                _dbContext.PotentialPositions.Update(pp);
               await  _dbContext.SaveChangesAsync();
            }
        }

        public async Task UnUpToDateAll()
        {
            foreach (var pp in await _dbContext.PotentialPositions.ToListAsync())
            {
                pp.IsUpToDate = false;
                _dbContext.PotentialPositions.Update(pp);
            }
            await _dbContext.SaveChangesAsync();
        }
    }
}

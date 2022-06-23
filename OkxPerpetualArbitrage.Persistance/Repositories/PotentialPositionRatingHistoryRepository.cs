using OkxPerpetualArbitrage.Application.Contracts.Persistance;
using OkxPerpetualArbitrage.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace OkxPerpetualArbitrage.Persistance.Repositories
{
    public class PotentialPositionRatingHistoryRepository : AsyncRepository<PotentialPositionRatingHistory>, IPotentialPositionRatingHistoryRepository
    {
        public PotentialPositionRatingHistoryRepository(SpotPerpDbContext spotPerpDbContext) :base(spotPerpDbContext)
        {

        }
    }
}

using OkxPerpetualArbitrage.Application.Contracts.Persistance;
using OkxPerpetualArbitrage.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace OkxPerpetualArbitrage.Persistance.Repositories
{
    public class PositionHistoryRepository : AsyncRepository<PositionHistory>, IPositionHistoryRepository
    {
        public PositionHistoryRepository(SpotPerpDbContext spotPerpDbContext) : base(spotPerpDbContext)
        {

        }
    }
}

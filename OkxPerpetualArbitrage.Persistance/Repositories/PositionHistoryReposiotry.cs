using OkxPerpetualArbitrage.Application.Contracts.Persistance;
using OkxPerpetualArbitrage.Domain.Entities;

namespace OkxPerpetualArbitrage.Persistance.Repositories
{
    public class PositionHistoryRepository : AsyncRepository<PositionHistory>, IPositionHistoryRepository
    {
        public PositionHistoryRepository(SpotPerpDbContext spotPerpDbContext) : base(spotPerpDbContext)
        {

        }
    }
}

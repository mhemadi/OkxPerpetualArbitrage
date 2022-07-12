using OkxPerpetualArbitrage.Application.Contracts.Persistance;
using OkxPerpetualArbitrage.Domain.Entities;

namespace OkxPerpetualArbitrage.Persistance.Repositories
{
    public class PotentialPositionRatingHistoryRepository : AsyncRepository<PotentialPositionRatingHistory>, IPotentialPositionRatingHistoryRepository
    {
        public PotentialPositionRatingHistoryRepository(SpotPerpDbContext spotPerpDbContext) : base(spotPerpDbContext)
        {

        }
    }
}

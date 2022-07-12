using OkxPerpetualArbitrage.Domain.Entities;

namespace OkxPerpetualArbitrage.Application.Contracts.Persistance
{
    public interface IPotentialPositionRatingHistoryRepository : IAsyncRepository<PotentialPositionRatingHistory>
    {
    }
}

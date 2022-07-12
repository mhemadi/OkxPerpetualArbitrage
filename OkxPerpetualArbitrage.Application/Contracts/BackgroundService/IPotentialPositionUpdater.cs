using OkxPerpetualArbitrage.Application.Contracts.OkxApi;
using OkxPerpetualArbitrage.Application.Contracts.Persistance;

namespace OkxPerpetualArbitrage.Application.Contracts.BackgroundService
{
    public interface IPotentialPositionUpdater : IBackgroundServiceTask
    {
        Task SavePotentialPositions(IOkxApiWrapper okxApiWrapper, IPotentialPositionRepository potentialPositionRepository, IPotentialPositionRatingHistoryRepository potentialPositionRatingHistoryRepository);
    }
}

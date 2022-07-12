using OkxPerpetualArbitrage.Application.Contracts.Persistance;

namespace OkxPerpetualArbitrage.Application.Contracts.BackgroundService
{
    public interface ICleaner : IBackgroundServiceTask
    {
        Task CleanOrphanDemands(IPositionDemandRepository positionDemandRepository);
        Task ExecuteAsync(CancellationToken stoppingToken);
    }
}

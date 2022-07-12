using OkxPerpetualArbitrage.Application.Contracts.BackgroundService;

namespace OkxPerpetualArbitrage.WebApi.BackgroundServices
{
    public class PotentialPositionUpdaterService : BackgroundService
    {
        private readonly IPotentialPositionUpdater _potentialPositionUpdater;

        public PotentialPositionUpdaterService(IPotentialPositionUpdater potentialPositionUpdater)
        {
            _potentialPositionUpdater = potentialPositionUpdater;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _potentialPositionUpdater.ExecuteAsync(stoppingToken);
        }
    }
}

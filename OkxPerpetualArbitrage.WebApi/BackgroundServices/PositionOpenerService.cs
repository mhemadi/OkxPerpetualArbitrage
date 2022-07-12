using OkxPerpetualArbitrage.Application.Contracts.BackgroundService;

namespace OkxPerpetualArbitrage.WebApi.BackgroundServices
{
    public class PositionOpenerService : BackgroundService
    {
        protected readonly IPositionOpener _positionOpener;

        public PositionOpenerService(IPositionOpener positionOpener)
        {
            _positionOpener = positionOpener;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _positionOpener.ExecuteAsync(stoppingToken);
        }
    }
}

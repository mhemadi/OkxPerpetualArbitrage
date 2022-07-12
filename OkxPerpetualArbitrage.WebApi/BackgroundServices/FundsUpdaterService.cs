using OkxPerpetualArbitrage.Application.Contracts.BackgroundService;

namespace OkxPerpetualArbitrage.WebApi.BackgroundServices
{
    /// <summary>
    /// This service gets the funding income records from the exchange and updates the db to keep the db uptodate with the the funding incomes earned.
    /// </summary>
    public class FundsUpdaterService : BackgroundService
    {
        private readonly IFundsUpdater _fundsUpdater;

        public FundsUpdaterService(IFundsUpdater fundsUpdater)
        {
            _fundsUpdater = fundsUpdater;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _fundsUpdater.ExecuteAsync(stoppingToken);
        }
    }
}

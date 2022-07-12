using OkxPerpetualArbitrage.Application.Contracts.BackgroundService;

namespace OkxPerpetualArbitrage.WebApi.BackgroundServices
{
    /// <summary>
    /// The cleaner service deletes 2 types of demands from the position demand repository
    /// Demands that are finished but were never able to get a fill on their orders. These records are deleted from the db since they are userless
    /// Demands that are still open after a few minutes. These demands should have been canceled but because of an internal error were left orphaned
    /// </summary>
    public class CleanerService : BackgroundService
    {
        private readonly ICleaner _cleaner;

        public CleanerService(ICleaner cleaner)
        {
            _cleaner = cleaner;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _cleaner.ExecuteAsync(stoppingToken);
        }
    }
}

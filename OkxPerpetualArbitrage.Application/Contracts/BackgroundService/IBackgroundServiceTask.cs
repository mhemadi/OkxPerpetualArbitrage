namespace OkxPerpetualArbitrage.Application.Contracts.BackgroundService
{
    public interface IBackgroundServiceTask
    {
        Task ExecuteAsync(CancellationToken stoppingToken);
    }
}

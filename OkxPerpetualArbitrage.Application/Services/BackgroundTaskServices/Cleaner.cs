using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OkxPerpetualArbitrage.Application.Contracts.BackgroundService;
using OkxPerpetualArbitrage.Application.Contracts.Persistance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OkxPerpetualArbitrage.Application.Services.BackgroundTaskServices
{


    public class Cleaner : ICleaner
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<Cleaner> _logger;
        private int counter = 0;
        private bool reset = false;

        public Cleaner(IServiceProvider serviceProvider, ILogger<Cleaner> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }
        public async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var positionDemandRepository = scope.ServiceProvider.GetRequiredService<IPositionDemandRepository>();
                        var potentialPositionRatingHistoryRepository = scope.ServiceProvider.GetRequiredService<IPotentialPositionRatingHistoryRepository>();
                        await CleanOrphanDemands(positionDemandRepository);
                        await CleanUnfilledDemands(positionDemandRepository);
                        await CleanOldRatingHistory(potentialPositionRatingHistoryRepository);
                    }

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed cleaning");
                }
                _logger.LogInformation("Saved cleaning");
                await Task.Delay(10 * 60 * 1000, stoppingToken);

            }
        }


        public async Task CleanUnfilledDemands(IPositionDemandRepository positionDemandRepository)
        {
            var unfilleds = await positionDemandRepository.GetUnfilledDemands();
            unfilleds = unfilleds.Where(x => x.UpdateDate < DateTime.UtcNow.AddDays(-2)).ToList();
            for (int i = 0; i < unfilleds.Count; i++)
            {
                _logger.LogWarning($"Found demand {unfilleds[i].PositionDemandId} to be old and not filled and gonna delete it");
                await positionDemandRepository.DeleteAsync(unfilleds[i]);
            }
        }

        public async Task CleanOldRatingHistory(IPotentialPositionRatingHistoryRepository  potentialPositionRatingHistoryRepository)
        {
            var olds = await potentialPositionRatingHistoryRepository.GetAllAsync();
            olds = olds.Where(x => x.Timestamp < DateTime.UtcNow.AddDays(-2)).ToList();
            for (int i = 0; i < olds.Count; i++)
            {             
                await potentialPositionRatingHistoryRepository.DeleteAsync(olds[i]);
            }
        }

        public async Task CleanOrphanDemands(IPositionDemandRepository positionDemandRepository)
        {
            var inprogress = await positionDemandRepository.GetInProgressDemands(Domain.Entities.Enums.PositionDemandSide.Open);
            inprogress.AddRange(await positionDemandRepository.GetInProgressDemands(Domain.Entities.Enums.PositionDemandSide.Close));
            inprogress = inprogress.Where(x => x.UpdateDate < DateTime.UtcNow.AddMinutes(-10)).ToList();
            foreach (var demand in inprogress)
            {
                if (!demand.IsCanceled)
                {
                    await positionDemandRepository.SetIsCanceled(demand.PositionDemandId, true);
                    _logger.LogWarning($"Found demand {demand.PositionDemandId} to be orphanded and canceled it");
                }
            }


            inprogress = await positionDemandRepository.GetInProgressDemands(Domain.Entities.Enums.PositionDemandSide.Open);
            inprogress.AddRange(await positionDemandRepository.GetInProgressDemands(Domain.Entities.Enums.PositionDemandSide.Close));
            var readyToDelete = inprogress.Where(x => x.UpdateDate < DateTime.UtcNow.AddMinutes(-5) && x.IsCanceled).ToList();

            for (int i = 0; i < readyToDelete.Count; i++)
            {
                _logger.LogWarning($"Found demand {readyToDelete[i].PositionDemandId} to be orphanded and gonna delete it");
                await positionDemandRepository.DeleteAsync(readyToDelete[i]);

            }


        }

    }
}

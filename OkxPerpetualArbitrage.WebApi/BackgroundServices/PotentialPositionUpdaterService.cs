using Microsoft.Extensions.Hosting;
using OkxPerpetualArbitrage.Application.Contracts.BackgroundService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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

using Microsoft.Extensions.Hosting;
using OkxPerpetualArbitrage.Application.Contracts.BackgroundService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OkxPerpetualArbitrage.WebApi.BackgroundServices
{
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

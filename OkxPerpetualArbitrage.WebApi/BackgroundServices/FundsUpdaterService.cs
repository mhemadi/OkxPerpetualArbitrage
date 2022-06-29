using Microsoft.Extensions.Hosting;
using OkxPerpetualArbitrage.Application.Contracts.BackgroundService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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

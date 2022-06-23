using Microsoft.Extensions.Hosting;
using OkxPerpetualArbitrage.Application.Contracts.BackgroundService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OkxPerpetualArbitrage.WebApi.BackgroundServices
{
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

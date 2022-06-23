using Microsoft.Extensions.Hosting;
using OkxPerpetualArbitrage.Application.Contracts.BackgroundService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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

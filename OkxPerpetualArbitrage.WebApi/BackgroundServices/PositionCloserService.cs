using Microsoft.Extensions.Hosting;
using OkxPerpetualArbitrage.Application.Contracts.BackgroundService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OkxPerpetualArbitrage.WebApi.BackgroundServices
{
    public class PositionCloserService : BackgroundService
    {
        private readonly IPositionCloser _positionCloser;

        public PositionCloserService(IPositionCloser positionCloser)
        {
            _positionCloser = positionCloser;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _positionCloser.ExecuteAsync(stoppingToken);
        }
    }
}

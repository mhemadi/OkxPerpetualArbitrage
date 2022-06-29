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
    /// This service processes close requests in the ClosePositionProcessingChannel and closes the symbols requested
    /// </summary>
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

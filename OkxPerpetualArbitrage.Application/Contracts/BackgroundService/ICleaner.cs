using OkxPerpetualArbitrage.Application.Contracts.Persistance;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OkxPerpetualArbitrage.Application.Contracts.BackgroundService
{
    public interface ICleaner : IBackgroundServiceTask
    {
        Task CleanOrphanDemands(IPositionDemandRepository positionDemandRepository);
        Task ExecuteAsync(CancellationToken stoppingToken);
    }
}

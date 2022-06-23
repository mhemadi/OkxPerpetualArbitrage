using MediatR;
using OkxPerpetualArbitrage.Application.Contracts.ApiService;
using OkxPerpetualArbitrage.Application.Contracts.Persistance;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OkxPerpetualArbitrage.Application.Contracts.BackgroundService
{
    public interface IAutoPositionOpener
    {
        Task AutoOpen(IApiService apiService, IPositionDemandRepository positionDemandRepository, IPotentialPositionRepository potentialPositionRepository, IOrderFillRepository orderFillRepository, IPotentialPositionRatingHistoryRepository potentialPositionRatingHistoryRepository, IMediator mediator);
        Task ExecuteAsync(CancellationToken stoppingToken);
    }
}

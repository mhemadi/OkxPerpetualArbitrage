using OkxPerpetualArbitrage.Application.Contracts.OkxApi;
using OkxPerpetualArbitrage.Application.Contracts.Persistance;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OkxPerpetualArbitrage.Application.Contracts.BackgroundService
{
    public interface IPotentialPositionUpdater : IBackgroundServiceTask
    {
        Task SavePotentialPositions(IOkxApiWrapper _apiService, IPotentialPositionRepository _potentialPositionRep, IPotentialPositionRatingHistoryRepository potentialPositionRatingHistoryRepository);
    }
}

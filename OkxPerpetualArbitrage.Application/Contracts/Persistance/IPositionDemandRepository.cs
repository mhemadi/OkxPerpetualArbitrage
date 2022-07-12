using OkxPerpetualArbitrage.Domain.Entities;
using OkxPerpetualArbitrage.Domain.Entities.Enums;

namespace OkxPerpetualArbitrage.Application.Contracts.Persistance
{
    public interface IPositionDemandRepository : IAsyncRepository<PositionDemand>
    {
        Task<bool> IsPositionDemandInProgress(string symbol);
        Task<bool> IsPositionDemandInProgress(string symbol, int excludeId);
        Task<List<PositionDemand>> GetInProgressDemandsBySymbolAndSide(string symbol, PositionDemandSide side);
        Task<List<PositionDemand>> GetInProgressDemandsBySymbol(string symbol);
        Task<List<PositionDemand>> GetInProgressDemands(PositionDemandSide side);
        Task<List<PositionDemand>> GetIncompleteDemands(string symbol);
        Task<List<PositionDemand>> GetIncompleteDemands();
        Task<int> SetIsCanceled(int positionDemandId, bool isCanceled);
        Task<int> SetFilled(int positionDemandId, decimal filled);
        Task<PositionDemand> GetPositionDemandNoTracking(int positionDemandId);
        Task<List<PositionDemand>> GetIncompleteDemandsNoTracking(string symbol);
        Task<List<PositionDemand>> GetUnfilledDemands();
    }
}

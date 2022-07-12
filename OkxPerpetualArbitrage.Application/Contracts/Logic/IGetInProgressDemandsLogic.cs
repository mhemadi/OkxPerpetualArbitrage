using OkxPerpetualArbitrage.Domain.Entities;

namespace OkxPerpetualArbitrage.Application.Contracts.Logic
{
    public interface IGetInProgressDemandsLogic
    {
        Task<PositionDemand> GetInProggressDemand(string symbol);
    }
}

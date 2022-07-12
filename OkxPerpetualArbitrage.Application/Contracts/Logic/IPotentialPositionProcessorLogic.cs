using OkxPerpetualArbitrage.Domain.Entities;

namespace OkxPerpetualArbitrage.Application.Contracts.Logic
{
    public interface IPotentialPositionProcessorLogic
    {
        Task<PotentialPosition> GetPotentialPosition(string symbol);
        Task<List<PotentialPosition>> GetAllPotentialPositions();
    }
}

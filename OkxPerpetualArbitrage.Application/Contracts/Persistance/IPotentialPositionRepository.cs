using OkxPerpetualArbitrage.Domain.Entities;

namespace OkxPerpetualArbitrage.Application.Contracts.Persistance
{
    public interface IPotentialPositionRepository : IAsyncRepository<PotentialPosition>
    {
        Task<PotentialPosition> GetPotentialPosition(string symbol);

        Task UnUpToDate(string symbol);

        Task UnUpToDateAll();
    }
}

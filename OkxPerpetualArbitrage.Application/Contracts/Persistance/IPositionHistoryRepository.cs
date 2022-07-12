using OkxPerpetualArbitrage.Domain.Entities;

namespace OkxPerpetualArbitrage.Application.Contracts.Persistance
{
    public interface IPositionHistoryRepository : IAsyncRepository<PositionHistory>
    {
    }
}

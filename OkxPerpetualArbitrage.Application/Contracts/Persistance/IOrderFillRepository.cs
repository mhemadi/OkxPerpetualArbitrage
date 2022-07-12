using OkxPerpetualArbitrage.Domain.Entities;

namespace OkxPerpetualArbitrage.Application.Contracts.Persistance
{
    public interface IOrderFillRepository : IAsyncRepository<OrderFill>
    {
        Task<List<OrderFill>> GetOrderFillsByPositionDemandId(int positionDemandId);
    }
}

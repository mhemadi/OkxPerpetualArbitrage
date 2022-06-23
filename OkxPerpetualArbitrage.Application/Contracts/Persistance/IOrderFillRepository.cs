using OkxPerpetualArbitrage.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OkxPerpetualArbitrage.Application.Contracts.Persistance
{
    public interface IOrderFillRepository : IAsyncRepository<OrderFill>
    {
        Task<List<OrderFill>> GetOrderFillsByPositionDemandId(int positionDemandId);
    }
}

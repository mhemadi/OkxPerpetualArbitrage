using Microsoft.EntityFrameworkCore;
using OkxPerpetualArbitrage.Application.Contracts.Persistance;
using OkxPerpetualArbitrage.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OkxPerpetualArbitrage.Persistance.Repositories
{
    class OrderFillRepository : AsyncRepository<OrderFill>, IOrderFillRepository
    {
        public OrderFillRepository(SpotPerpDbContext spotPerpDbContext) : base(spotPerpDbContext)
        {
        }

        public async Task<List<OrderFill>> GetOrderFillsByPositionDemandId(int positionDemandId)
        {
            return await _dbContext.OrderFills.Where(x => x.PositionDemand.PositionDemandId == positionDemandId).ToListAsync();
        }
    }
}

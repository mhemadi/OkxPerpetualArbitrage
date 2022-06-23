using OkxPerpetualArbitrage.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace OkxPerpetualArbitrage.Application.Contracts.Persistance
{
    public interface IPotentialPositionRatingHistoryRepository : IAsyncRepository<PotentialPositionRatingHistory>
    {
    }
}

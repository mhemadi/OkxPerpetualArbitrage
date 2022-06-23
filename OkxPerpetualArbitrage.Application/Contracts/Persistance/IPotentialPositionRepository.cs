using OkxPerpetualArbitrage.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OkxPerpetualArbitrage.Application.Contracts.Persistance
{
    public interface IPotentialPositionRepository : IAsyncRepository<PotentialPosition>
    {
        Task<PotentialPosition> GetPotentialPosition(string symbol);

        Task UnUpToDate(string symbol);

        Task UnUpToDateAll();
    }
}

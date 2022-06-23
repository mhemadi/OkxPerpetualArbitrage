using OkxPerpetualArbitrage.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OkxPerpetualArbitrage.Application.Contracts.Logic
{
    public interface IPositionOpenLogic
    {
        Task Open(string symbol, int positionDemandId, decimal lotSize, decimal lotSizeChunk, decimal minSpread, PotentialPosition potentialPosition);
    }
}

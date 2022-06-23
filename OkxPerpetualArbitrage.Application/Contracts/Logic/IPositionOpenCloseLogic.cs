using OkxPerpetualArbitrage.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OkxPerpetualArbitrage.Application.Contracts.Logic
{
    public interface IPositionOpenCloseLogic
    {
        Task Close(string symbol, int positionDemandId, decimal lotSize, decimal lotSizeChunk, decimal minSpread, PotentialPosition potentialPosition, bool isInstant);
        Task Open(string symbol, int positionDemandId, decimal lotSize, decimal lotSizeChunk, decimal minSpread, PotentialPosition potentialPosition);
    }
}

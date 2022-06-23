using OkxPerpetualArbitrage.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OkxPerpetualArbitrage.Application.Contracts.Logic
{
    public interface IPositionChunkCreateLogic
    {
        Task<decimal> OpenClosePositionChunck(string symbol, decimal lotSizeChunk, int positionDemandId, decimal minSpread, PotentialPosition potentialPosition, bool tryToBeMaker, bool open);
    }
}

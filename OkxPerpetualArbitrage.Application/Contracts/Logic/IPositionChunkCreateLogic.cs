using OkxPerpetualArbitrage.Domain.Entities;

namespace OkxPerpetualArbitrage.Application.Contracts.Logic
{
    public interface IPositionChunkCreateLogic
    {
        Task<decimal> OpenClosePositionChunck(string symbol, decimal lotSizeChunk, int positionDemandId, decimal minSpread, PotentialPosition potentialPosition, bool tryToBeMaker, bool open);
    }
}

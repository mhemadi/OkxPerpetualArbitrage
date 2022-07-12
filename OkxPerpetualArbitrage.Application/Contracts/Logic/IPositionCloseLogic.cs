using OkxPerpetualArbitrage.Domain.Entities;

namespace OkxPerpetualArbitrage.Application.Contracts.Logic
{
    public interface IPositionCloseLogic
    {
        Task Close(string symbol, int positionDemandId, decimal lotSize, decimal lotSizeChunk, decimal minSpread, PotentialPosition potentialPosition, bool isInstant);
        Task<decimal> ClosePositionInstant(string symbol, decimal lotSize, int positionDemandId, PotentialPosition potentialPosition);
    }
}

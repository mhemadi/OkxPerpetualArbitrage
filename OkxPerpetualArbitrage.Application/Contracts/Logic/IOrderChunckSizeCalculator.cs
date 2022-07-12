using OkxPerpetualArbitrage.Domain.Entities;

namespace OkxPerpetualArbitrage.Application.Contracts.Logic
{
    public interface IPositionChunckSizeCalculator
    {
        decimal GetChunkLotSize(PotentialPosition pp, decimal lotSize);
    }
}

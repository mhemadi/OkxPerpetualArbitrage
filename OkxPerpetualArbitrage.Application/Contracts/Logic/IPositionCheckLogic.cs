using OkxPerpetualArbitrage.Domain.Entities;

namespace OkxPerpetualArbitrage.Application.Contracts.Logic
{
    public interface IPositionCheckLogic
    {
        Task Checkposition(string symbol, PotentialPosition pp);
    }
}

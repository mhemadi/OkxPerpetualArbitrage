using OkxPerpetualArbitrage.Domain.Entities;

namespace OkxPerpetualArbitrage.Application.Contracts.Logic
{
    public interface IOrderCreateLogic
    {
        Task<string> CreateAdditionalSpotOrder(string symbol, PotentialPosition pp, decimal spotSize, bool tryToBeMaker, bool open);
        Task<Tuple<string, decimal>> CreateFirstSpotOrder(string symbol, decimal perpOrderSize, PotentialPosition pp, bool tryToBeMaker, bool open);
        Task<string> CreatePerpOrder(string symbol, decimal spreadThreshold, decimal size, PotentialPosition pp, bool tryToBeMaker, bool open);
    }
}

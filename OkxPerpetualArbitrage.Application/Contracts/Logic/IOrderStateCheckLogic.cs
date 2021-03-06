using OkxPerpetualArbitrage.Application.Models.OkexApi;
using OkxPerpetualArbitrage.Domain.Entities;

namespace OkxPerpetualArbitrage.Application.Contracts.Logic
{
    public interface IOrderStateCheckLogic
    {
        Task<bool> CanKeepOrderPerp(string symbol, OKEXOrder perpOrder, decimal spreadThreshold, bool open);
        Task<bool> IsSpotAboveMid(string symbol, OKEXOrder spotOrder, PotentialPosition pp);
        Task<bool> IsSpotBelowMid(string symbol, OKEXOrder spotOrder, PotentialPosition pp);
    }
}

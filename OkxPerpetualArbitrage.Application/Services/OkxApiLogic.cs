using OkxPerpetualArbitrage.Application.Contracts.Logic;
using OkxPerpetualArbitrage.Application.Contracts.OkxApi;

namespace OkxPerpetualArbitrage.Application.Services
{
    public class OkxApiLogic : IOkxApiLogic
    {
        private readonly IOkxApiWrapper _okxApi;

        public OkxApiLogic(IOkxApiWrapper okxApi)
        {
            _okxApi = okxApi;
        }
        public async Task<decimal> GetAvailableUsdtBalance()
        {
            return (await _okxApi.GetUSDTBalance()).Available;
        }
    }
}

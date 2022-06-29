using OkxPerpetualArbitrage.Application.Contracts.OkxApi;
using OkxPerpetualArbitrage.Application.Contracts.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

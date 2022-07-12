using OkxPerpetualArbitrage.Application.Models.OkexApi;
using OkxPerpetualArbitrage.Application.Models.OkexApi.Enums;

namespace OkxPerpetualArbitrage.Application.Contracts.OkxApi
{
    public interface IOkxApiWrapper
    {
        void SetMaxTry(int maximumTries);
        void SetWait(int waitMiliSeconds);
        Task<bool> CancelOrder(string instrument, string orderId);
        Task<List<OKEXMarketInfo>> GetAllSymbols();
        Task<Tuple<decimal, decimal>> GetFundingRates(string symbol);
        Task<OKEXOrder> GetOrder(string instrument, string orderId);
        Task<OrderBook> GetOrderBook(string instrumnet);
        Task<Tuple<decimal, decimal, decimal>> GetPerpContractData(string symbol);
        string GetPerpInstrument(string symbol);
        Task<List<OKEXBill>> GetRecentFundingBills();
        Task<Tuple<decimal, decimal, decimal, decimal>> GetSpotAndPerpMakerAndTakerFees(string symbol);
        Task<Tuple<decimal, decimal, decimal>> GetSpotContractData(string symbol);
        string GetSpotInstrument(string symbol);
        Task<OKEXBalance> GetUSDTBalance();
        Task<OKEXBalance> GetBalance(string symbol);
        Task<string> PlaceOrder(string instId, OKEXOrderType orderType, OKEXTadeMode tdMode, OKEXOrderSide side, OKEXPostitionSide posSide, decimal size, decimal px, bool reduceOnly);
        Task<decimal> GetPositionSize(string symbol);
        Task<int> GetMinutesToMaintenace();
        Task<List<decimal>> GetFundingHistory(string symbol, int days);
    }
}

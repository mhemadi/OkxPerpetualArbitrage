using OkxPerpetualArbitrage.Application.Models.OkexApi;
using OkxPerpetualArbitrage.Application.Models.OkexApi.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OkxPerpetualArbitrage.Application.Contracts.OkxApi
{
    public interface IOkxApiWrapper
    {
        Task<bool> CancelOrder(string instrument, string orderId, int maximumTries = 0, int waitMiliSeconds = 0);
        Task<List<OKEXMarketInfo>> GetAllSymbols(int maximumTries = 0, int waitMiliSeconds = 0);
        Task<Tuple<decimal, decimal>> GetFundingRates(string symbol, int maximumTries = 0, int waitMiliSeconds = 0);
        Task<OKEXOrder> GetOrder(string instrument, string orderId, int maximumTries = 0, int waitMiliSeconds = 0);
        Task<OrderBook> GetOrderBook(string instrumnet, int maximumTries = 0, int waitMiliSeconds = 0);
        Task<Tuple<decimal, decimal, decimal>> GetPerpContractData(string symbol, int maximumTries = 0, int waitMiliSeconds = 0);
        string GetPerpInstrument(string symbol);
        Task<List<OKEXBill>> GetRecentFundingBills(int maximumTries = 0, int waitMiliSeconds = 0);
        Task<Tuple<decimal, decimal, decimal, decimal>> GetSpotAndPerpMakerAndTakerFees(string symbol, int maximumTries = 0, int waitMiliSeconds = 0);
        Task<Tuple<decimal, decimal, decimal>> GetSpotContractData(string symbol, int maximumTries = 0, int waitMiliSeconds = 0);
        string GetSpotInstrument(string symbol);
        Task<OKEXBalance> GetUSDTBalance(int maximumTries = 0, int waitMiliSeconds = 0);
        Task<OKEXBalance> GetBalance(string symbol, int maximumTries = 0, int waitMiliSeconds = 0);
        Task<string> PlaceOrder(string instId, OKEXOrderType orderType, OKEXTadeMode tdMode, OKEXOrderSide side, OKEXPostitionSide posSide, decimal size, decimal px, bool reduceOnly, int maximumTries = 0, int waitMiliSeconds = 0);
        Task<decimal> GetPositionSize(string symbol, int maximumTries = 0, int waitMiliSeconds = 0);
        Task<int> GetMinutesToMaintenace(int maximumTries = 0, int waitMiliSeconds = 0);
        Task<List<decimal>> GetFundingHistory(string symbol, int days, int maximumTries = 0, int waitMiliSeconds = 0);
    }
}

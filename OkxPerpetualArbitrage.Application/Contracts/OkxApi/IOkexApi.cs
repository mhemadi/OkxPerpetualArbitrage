using OkxPerpetualArbitrage.Application.Models.OkexApi;
using OkxPerpetualArbitrage.Application.Models.OkexApi.Enums;

namespace OkxPerpetualArbitrage.Application.Contracts.OkxApi
{
    public interface IOkexApi
    {
        Task<OKEXResponse<bool>> CancelOrder(string instrument, string cOrderId);
        Task<OKEXResponse<List<OKEXOrder>>> GetArchivedOrders(OKEXInstrumentType instrumentType, string instrument);
        Task<OKEXResponse<List<OKEXBalance>>> GetBalances();
        Task<OKEXResponse<Tuple<decimal, decimal, DateTime>>> GetCurrentAndNextFundingRatesAndFundingTime(string instrument);
        Task<OKEXResponse<OrderBook>> GetCurrentOrderBook(string instrument);
        Task<OKEXResponse<Tuple<decimal, decimal>>> GetFeeForMakerAndTaker(OKEXInstrumentType instrumentType, string instrument);
        Task<OKEXResponse<List<OKEXMarketInfo>>> GetInstruments(OKEXInstrumentType instrumentType, string underlying = "");
        Task<OKEXResponse<decimal>> GetInterestFreeAmount(string currency);
        Task<OKEXResponse<decimal>> GetInterestRate(string currency);
        Task<OKEXResponse<Tuple<DateTime, DateTime>>> GetMaintanaceTime();
        Task<OKEXResponse<List<OKEXOrder>>> GetOpenOrders(OKEXInstrumentType instrumentType, string instrument);
        Task<OKEXResponse<OKEXOrder>> GetOrderByClientOrderId(string instrument, string cOrderId);
        Task<OKEXResponse<OKEXOrder>> GetOrderByOKEXOrderId(string instrument, string okexOrderId);
        Task<OKEXResponse<Tuple<decimal, decimal, decimal>>> GetPerpContractMinSizeAndTickSizeAndContractValue(string instrument);
        Task<OKEXResponse<decimal>> GetPositionSize(OKEXInstrumentType instrumentType, string instrument);
        Task<OKEXResponse<List<OKEXOrder>>> GetRecentsOrders(OKEXInstrumentType instrumentType, string instrument);
        Task<OKEXResponse<decimal>> GetSpotMinSize(string instrument);
        Task<OKEXResponse<Tuple<decimal, decimal, decimal>>> GetSpotMinSizeAndLotSizeAndTickSize(string instrument);
        Task<OKEXResponse<bool>> PlaceLimitOrder(string instId, OKEXTadeMode tdMode, string clOrdId, OKEXOrderSide side, decimal size, decimal px, bool reduceOnly);
        Task<OKEXResponse<bool>> PlaceLimitOrderPosition(string instId, OKEXTadeMode tdMode, string clOrdId, OKEXOrderSide side, string posSide, decimal size, decimal px, bool reduceOnly);
        Task<OKEXResponse<bool>> PlaceMarketOrder(string instId, OKEXTadeMode tdMode, string clOrdId, OKEXOrderSide side, decimal size, bool reduceOnly);
        Task<OKEXResponse<bool>> PlaceMarketOrderPosition(string instId, OKEXTadeMode tdMode, string clOrdId, OKEXOrderSide side, string posSide, decimal size, bool reduceOnly);
        Task<OKEXResponse<List<decimal>>> GetFundingIncomeHistory(string instrument);
        Task<OKEXResponse<List<OKEXBill>>> GetFundingBills();
    }
}

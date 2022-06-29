using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OkxPerpetualArbitrage.Application.Contracts.OkxApi;
using OkxPerpetualArbitrage.Application.Helpers;
using OkxPerpetualArbitrage.Application.Models.InfrastructureSettings;
using OkxPerpetualArbitrage.Application.Models.OkexApi;
using OkxPerpetualArbitrage.Application.Models.OkexApi.Enums;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace OkxPerpetualArbitrage.OkxApi
{


    public class OKEXV5Api : IOkexApi
    {
        private const string Url = "https://www.okex.com/";
        private string apiKey;
        private string apiSecret;
        private string passPhrase;
        private readonly OkexApiSetting _setting;
        private readonly ILogger _logger;
        private readonly HttpClient _httpClient;

        public OKEXV5Api(IOptions<OkexApiSetting> setting, ILogger<OKEXV5Api> logger)
        {
            _setting = setting.Value;
            this.apiKey = _setting.ApiKey;
            this.apiSecret = _setting.ApiSecret;
            this.passPhrase = _setting.ApiPassPhrase;
            _logger = logger;
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(Url),
                Timeout = TimeSpan.FromSeconds(3)
            };

        }

        private async Task<OKEXResponse<dynamic>> CallApi(string url, HttpMethod httpMethod)
        {
            var result = await CallAsyncSign(httpMethod, url);
            dynamic o = JsonConvert.DeserializeObject(result);
            return new OKEXResponse<dynamic>()
            {
                Code = Convert.ToInt32(o.code),
                Message = Convert.ToString(o.msg),
                Data = o.data
            };
        }
        private decimal GetDecimal(object o)
        {

            if (o == null || string.IsNullOrEmpty(o.ToString()))
                return 0;
            return Convert.ToDecimal(o);
        }



        public async Task<OKEXResponse<decimal>> GetInterestFreeAmount(string currency)
        {
            var url = $"/api/v5/public/discount-rate-interest-free-quota?ccy={currency}";
            var apiResponse = await CallApi(url, HttpMethod.Get);
            var result = new OKEXResponse<decimal>()
            {
                Code = apiResponse.Code,
                Data = 0,
                Message = apiResponse.Message
            };
            if (apiResponse.Code != 0)
                return result;

            int i = 0;
            foreach (var v in apiResponse.Data)
                i++;
            if (i != 1)
                return result;
            result.Data = Convert.ToDecimal(apiResponse.Data[0].amt);
            return result;
        }

        public async Task<OKEXResponse<decimal>> GetInterestRate(string currency)
        {
            var url = $"/api/v5/account/interest-rate?ccy={currency}";
            var apiResponse = await CallApi(url, HttpMethod.Get);
            var result = new OKEXResponse<decimal>()
            {
                Code = apiResponse.Code,
                Data = 0,
                Message = apiResponse.Message
            };
            if (apiResponse.Code != 0)
                return result;

            int i = 0;
            foreach (var v in apiResponse.Data)
                i++;
            if (i != 1)
                return result;
            result.Data = Convert.ToDecimal(apiResponse.Data[0].interestRate);
            return result;
        }
        public async Task<OKEXResponse<List<OKEXBalance>>> GetBalances()
        {
            var url = $"/api/v5/account/balance";
            var apiResponse = await CallApi(url, HttpMethod.Get);
            var result = new OKEXResponse<List<OKEXBalance>>()
            {
                Code = apiResponse.Code,
                Data = null,
                Message = apiResponse.Message
            };
            if (apiResponse.Code != 0)
                return result;
            result.Data = new List<OKEXBalance>();
            foreach (var item in apiResponse.Data[0].details)
            {
                result.Data.Add(new OKEXBalance() { Symbol = Convert.ToString(item.ccy), Equity = Convert.ToDecimal(item.availEq), Cash = Convert.ToDecimal(item.cashBal) });
            }
            return result;
        }


        public async Task<OKEXResponse<decimal>> GetPositionSize(OKEXInstrumentType instrumentType, string instrument)
        {
            var url = $"/api/v5/account/positions?instType={instrumentType}&instId={instrument}";
            var apiResponse = await CallApi(url, HttpMethod.Get);
            var result = new OKEXResponse<decimal>()
            {
                Code = apiResponse.Code,
                Data = 0,
                Message = apiResponse.Message
            };
            if (apiResponse.Code != 0)
                return result;

            int i = 0;
            foreach (var v in apiResponse.Data)
                i++;
            if (i != 1)
                return result;
            result.Data = Convert.ToDecimal(apiResponse.Data[0].pos);
            return result;
        }
        /*
        public async Task<OrderBookItem> GetPositionSimpledata(OKEXInstrumentType instrumentType, string instrument)
        {
            var resultString = $"/api/v5/account/positions?instType={instrumentType}&instId={instrument}";
            var result = await CallAsyncSign(HttpMethod.Get, resultString);
            dynamic o = JsonConvert.DeserializeObject(result);
            if (o.code != 0)
                return null;
            int i = 0;
            foreach (var v in o.data)
                i++;
            if (i == 0 || (o.data[0].pos == 0))
            {
                return new OrderBookItem() { Amount = 0, Price = -1 };
            }
            if (i != 1)
                return null;
            OrderBookItem obi = new OrderBookItem();
            obi.Amount = o.data[0].pos;
            obi.Price = o.data[0].avgPx;
            return obi;
        }

        public async Task<decimal> GetPositionLiability(OKEXInstrumentType instrumentType, string instrument)
        {
            var resultString = $"/api/v5/account/positions?instType={instrumentType}&instId={instrument}";
            var result = await CallAsyncSign(HttpMethod.Get, resultString);
            dynamic o = JsonConvert.DeserializeObject(result);
            if (o.code != 0)
                return 1;
            int i = 0;
            foreach (var v in o.data)
                i++;
            if (i != 1)
                return 1;
            decimal l = o.data[0].liab;
            return l;
        }
        public async Task<decimal> GetPositionLiquidationPrice(OKEXInstrumentType instrumentType, string instrument)
        {
            var resultString = $"/api/v5/account/positions?instType={instrumentType}&instId={instrument}";
            var result = await CallAsyncSign(HttpMethod.Get, resultString);
            dynamic o = JsonConvert.DeserializeObject(result);
            if (o.code != 0)
                return -1;
            int i = 0;
            foreach (var v in o.data)
                i++;
            if (i != 1)
                return -1;
            decimal l = o.data[0].liqPx;
            return l;
        }
        */
        public async Task<OKEXResponse<Tuple<decimal, decimal>>> GetFeeForMakerAndTaker(OKEXInstrumentType instrumentType, string instrument)
        {

            var url = $"/api/v5/account/trade-fee?instType={instrumentType}&instId={instrument}";
            if (instrumentType != OKEXInstrumentType.SPOT)
                url += "&category=1";
            var apiResponse = await CallApi(url, HttpMethod.Get);
            var result = new OKEXResponse<Tuple<decimal, decimal>>()
            {
                Code = apiResponse.Code,
                Data = null,
                Message = apiResponse.Message
            };
            if (apiResponse.Code != 0)
                return result;

            int i = 0;
            foreach (var v in apiResponse.Data)
                i++;
            if (i != 1)
                return result;
            result.Data = new Tuple<decimal, decimal>(Convert.ToDecimal(apiResponse.Data[0].maker), Convert.ToDecimal(apiResponse.Data[0].taker));
            return result;
        }

        public async Task<OKEXResponse<Tuple<decimal, decimal, DateTime>>> GetCurrentAndNextFundingRatesAndFundingTime(string instrument)
        {
            var url = $"/api/v5/public/funding-rate?instId={instrument}";
            var apiResponse = await CallApi(url, HttpMethod.Get);
            var result = new OKEXResponse<Tuple<decimal, decimal, DateTime>>()
            {
                Code = apiResponse.Code,
                Data = null,
                Message = apiResponse.Message
            };
            if (apiResponse.Code != 0)
                return result;

            int i = 0;
            foreach (var v in apiResponse.Data)
                i++;
            if (i != 1)
                return result;
            result.Data = new Tuple<decimal, decimal, DateTime>(Convert.ToDecimal(apiResponse.Data[0].fundingRate), Convert.ToDecimal(apiResponse.Data[0].nextFundingRate), DateTimeHelper.GetDateFromUnix(Convert.ToInt64(apiResponse.Data[0].fundingTime)));
            return result;
        }
        /*
        public async Task<decimal> GetOptionExcercisePrice(string currency, string instrument)
        {
            var resultString = $"/api/v5/public/delivery-exercise-history?instType=OPTION&uly={currency}-USD";
            var result = await CallAsyncSign(HttpMethod.Get, resultString);
            dynamic o = JsonConvert.DeserializeObject(result);
            if (o.code != 0)
                return -1;
            foreach (var v in o.data[0].details)
            {
                string inst = Convert.ToString(v.insId);
                if (inst == instrument)
                    return GetDecimal(v.px);
            }
            return -1;
        }

        public async Task<Tuple<decimal, decimal>> GetPerpContractValueAndTickSize(string instrument, OKEXInstrumentType instrumentType = OKEXInstrumentType.SWAP)
        {
            var resultString = $"/api/v5/public/instruments?instType={instrumentType}&instId={instrument}";
            var result = await CallAsyncSign(HttpMethod.Get, resultString);
            dynamic o = JsonConvert.DeserializeObject(result);
            if (o.code != 0)
                return new Tuple<decimal, decimal>(0, 0);
            int i = 0;
            foreach (var v in o.data)
                i++;
            if (i != 1)
                return new Tuple<decimal, decimal>(0, 0);
            return new Tuple<decimal, decimal>(Convert.ToDecimal(o.data[0].ctVal), Convert.ToDecimal(o.data[0].tickSz));

        }
        */
        public async Task<OKEXResponse<Tuple<decimal, decimal, decimal>>> GetPerpContractMinSizeAndTickSizeAndContractValue(string instrument)
        {
            var url = $"/api/v5/public/instruments?instType={OKEXInstrumentType.SWAP}&instId={instrument}";
            var apiResponse = await CallApi(url, HttpMethod.Get);
            var result = new OKEXResponse<Tuple<decimal, decimal, decimal>>()
            {
                Code = apiResponse.Code,
                Data = null,
                Message = apiResponse.Message
            };
            if (apiResponse.Code != 0)
                return result;

            int i = 0;
            foreach (var v in apiResponse.Data)
                i++;
            if (i != 1)
                return result;
            result.Data = new Tuple<decimal, decimal, decimal>(Convert.ToDecimal(apiResponse.Data[0].minSz), Convert.ToDecimal(apiResponse.Data[0].tickSz), Convert.ToDecimal(apiResponse.Data[0].ctVal));
            return result;
        }
        public async Task<OKEXResponse<Tuple<decimal, decimal, decimal>>> GetSpotMinSizeAndLotSizeAndTickSize(string instrument)
        {
            var url = $"/api/v5/public/instruments?instType={OKEXInstrumentType.SPOT}&instId={instrument}";
            var apiResponse = await CallApi(url, HttpMethod.Get);
            var result = new OKEXResponse<Tuple<decimal, decimal, decimal>>()
            {
                Code = apiResponse.Code,
                Data = null,
                Message = apiResponse.Message
            };
            if (apiResponse.Code != 0)
                return result;

            int i = 0;
            foreach (var v in apiResponse.Data)
                i++;
            if (i != 1)
                return result;
            result.Data = new Tuple<decimal, decimal, decimal>(Convert.ToDecimal(apiResponse.Data[0].minSz), Convert.ToDecimal(apiResponse.Data[0].lotSz), Convert.ToDecimal(apiResponse.Data[0].tickSz));
            return result;

        }

        public async Task<OKEXResponse<decimal>> GetSpotMinSize(string instrument)
        {
            var url = $"/api/v5/public/instruments?instType=SPOT&instId={instrument}";
            var apiResponse = await CallApi(url, HttpMethod.Get);
            var result = new OKEXResponse<decimal>()
            {
                Code = apiResponse.Code,
                Data = 0,
                Message = apiResponse.Message
            };
            if (apiResponse.Code != 0)
                return result;

            int i = 0;
            foreach (var v in apiResponse.Data)
                i++;
            if (i != 1)
                return result;
            result.Data = Convert.ToDecimal(apiResponse.Data[0].minSz);
            return result;

        }
     
      public async Task<OKEXResponse<List<OKEXBill>>> GetFundingBills()
      {
          return await GetBill(8);
      }
      public async Task<OKEXResponse<List<OKEXBill>>> GetDeliveryBills()
      {
          return await GetBill(3);
      }
      private async Task<OKEXResponse<List<OKEXBill>>> GetBill(int type)
      {
            var url = $"/api/v5/account/bills?type={type}";
            var apiResponse = await CallApi(url, HttpMethod.Get);
            var result = new OKEXResponse<List<OKEXBill>>(){Code = apiResponse.Code,Data = null,Message = apiResponse.Message};
            if (apiResponse.Code != 0)
                return result;
            result.Data = new List<OKEXBill>();
            foreach (var item in apiResponse.Data)
            {
                result.Data.Add(new OKEXBill()
                {
                    BalanceChange = Convert.ToDecimal(item.balChg),
                    BillId = item.billId,
                    Instrument = item.instId,
                    BillType = Convert.ToString(item.type),
                    Currency = item.ccy,
                    TimeStamp = DateTimeHelper.GetDateFromUnix(Convert.ToInt64(item.ts))
                });
            }
            return result;
      }
      
      public async Task<OKEXResponse<List<decimal>>> GetFundingIncomeHistory(string instrument)
      {
            var url = $"/api/v5/public/funding-rate-history?instId={instrument}";
            var apiResponse = await CallApi(url, HttpMethod.Get);
            var result = new OKEXResponse<List<decimal>>() { Code = apiResponse.Code, Data = null, Message = apiResponse.Message };
            if (apiResponse.Code != 0)
                return result;
            result.Data = new List<decimal>();
            foreach (var item in apiResponse.Data)
            {
                result.Data.Add(Convert.ToDecimal(item.realizedRate));
            }
            return result;
      }
        /*
     public async Task<List<OKEXFundingIncome>> GetFundingIncome(long from)
   {
       var resultString = $"/api/v5/account/bills?type=8&before={from}";
       var result = await CallAsyncSign(HttpMethod.Get, resultString);
       dynamic o = JsonConvert.DeserializeObject(result);
       if (o.code != 0)
           return null;
       List<OKEXFundingIncome> incomes = new List<OKEXFundingIncome>();
       foreach (var item in o.data)
       {
           incomes.Add(new OKEXFundingIncome()
           {
               Amount = Convert.ToDecimal(item.pnl),
               BillId = Convert.ToInt64(item.billId),
               Instrument = item.instId,
               MarginMode = item.mgnMode,
               Timestamp = DateTimeHelper.GetDateFromUnix(Convert.ToInt64(item.ts))
           });
       }
       return incomes;
   }

   public async Task<List<string>> GetOptionContracts(string currency, string date)
   {
       var resultString = $"/api/v5/public/opt-summary?uly={currency}-USD&expTime={date}";

       var result = await CallAsyncSign(HttpMethod.Get, resultString);
       dynamic o = JsonConvert.DeserializeObject(result);
       if (o.code != 0)
           return null;

       List<string> instruments = new List<string>();
       foreach (var item in o.data)
       {
           instruments.Add(Convert.ToString(item.instId));
       }
       return instruments;
   }
   */
        public async Task<OKEXResponse<List<OKEXMarketInfo>>> GetInstruments(OKEXInstrumentType instrumentType, string underlying = "")
        {
            var url = $"/api/v5/market/tickers?instType={instrumentType}";
            if (!string.IsNullOrEmpty(underlying))
                url = $"/api/v5/market/tickers?instType={instrumentType}&uly={underlying}-USD";
            var apiResponse = await CallApi(url, HttpMethod.Get);
            var result = new OKEXResponse<List<OKEXMarketInfo>>()
            {
                Code = apiResponse.Code,
                Data = null,
                Message = apiResponse.Message
            };
            if (apiResponse.Code != 0)
                return result;
            result.Data = new List<OKEXMarketInfo>();
            foreach (var item in apiResponse.Data)
            {
                result.Data.Add(new OKEXMarketInfo()
                {
                    SpotAsk = GetDecimal(item.askPx),
                    SpotBid = GetDecimal(item.bidPx),
                    Instrument = item.instId,
                    InstrumentType = instrumentType
                });
            }
            return result;
        }
        /*
        public async Task<List<OKEXInterestAccured>> GetInterestAccured(string instrument, DateTime fromDate)
        {
            var resultString = $"/api/v5/account/interest-accrued?instId={instrument}&before={DateTimeHelper.GetMillisecondsFromEpochStart(fromDate)}";
            var result = await CallAsyncSign(HttpMethod.Get, resultString);
            dynamic o = JsonConvert.DeserializeObject(result);
            if (o.code != 0)
                return null;
            List<OKEXInterestAccured> interests = new List<OKEXInterestAccured>();
            foreach (var item in o.data)
            {
                interests.Add(new OKEXInterestAccured()
                {
                    Currency = item.ccy,
                    Instrument = item.instId,
                    Interest = item.interest,
                    InterestRateHourly = item.interestRate,
                    Liability = item.liab,
                    MarginMode = item.mgnMode,
                    TimeStamp = DateTimeHelper.GetDateFromUnix(Convert.ToInt64(item.ts)),
                });
            }
            return interests;
        }
        */
        private bool HasData(dynamic data)
        {
            int i = 0;
            foreach (var v in data)
                i++;
            if (i != 1)
                return false;
            return true;
        }
        public async Task<OKEXResponse<OKEXOrder>> GetOrderByClientOrderId(string instrument, string cOrderId)
        {
            var url = $"/api/v5/trade/order?instId=" + instrument + "&clOrdId=" + cOrderId;
            var apiResponse = await CallApi(url, HttpMethod.Get);
            var result = new OKEXResponse<OKEXOrder>() { Code = apiResponse.Code, Data = null, Message = apiResponse.Message };
            if (apiResponse.Code != 0 || !HasData(apiResponse.Data))
                return result;
            result.Data = FillOrder(apiResponse.Data[0]);
            return result;
        }
        public async Task<OKEXResponse<OKEXOrder>> GetOrderByOKEXOrderId(string instrument, string okexOrderId)
        {
            var url = $"/api/v5/trade/order?instId=" + instrument + "&ordId=" + okexOrderId;
            var apiResponse = await CallApi(url, HttpMethod.Get);
            var result = new OKEXResponse<OKEXOrder>() { Code = apiResponse.Code, Data = null, Message = apiResponse.Message };
            if (apiResponse.Code != 0 || !HasData(apiResponse.Data))
                return result;
            result.Data = FillOrder(apiResponse.Data[0]);
            return result;
        }
        public async Task<OKEXResponse<List<OKEXOrder>>> GetArchivedOrders(OKEXInstrumentType instrumentType, string instrument)
        {
            return await GetOrders(instrumentType, instrument, "orders-history-archive");
        }
        public async Task<OKEXResponse<List<OKEXOrder>>> GetRecentsOrders(OKEXInstrumentType instrumentType, string instrument)
        {
            return await GetOrders(instrumentType, instrument, "orders-history");
        }
        public async Task<OKEXResponse<List<OKEXOrder>>> GetOpenOrders(OKEXInstrumentType instrumentType, string instrument)
        {
            return await GetOrders(instrumentType, instrument, "orders-pending");
        }
        private async Task<OKEXResponse<List<OKEXOrder>>> GetOrders(OKEXInstrumentType instrumentType, string instrument, string endPoint)
        {
            var url = $"/api/v5/trade/{endPoint}?instType={instrumentType.ToString()}&instId={instrument}";
            var apiResponse = await CallApi(url, HttpMethod.Get);
            var result = new OKEXResponse<List<OKEXOrder>>() { Code = apiResponse.Code, Data = null, Message = apiResponse.Message };
            if (apiResponse.Code != 0)
                return result;
            result.Data = new List<OKEXOrder>();
            foreach (var item in apiResponse.Data)
            {
                result.Data.Add(FillOrder(item));
            }
            return result;
        }

        private decimal GetFromDynamic(dynamic field)
        {
            string fieldStr = Convert.ToString(field);
            if (string.IsNullOrEmpty(fieldStr))
                return 0;
            return Convert.ToDecimal(fieldStr);
        }
        private OKEXOrder FillOrder(dynamic item)
        {
            var order = new OKEXOrder()
            {
                Size = item.accFillSz,
                InitialSize = item.sz,
                Price = GetFromDynamic(item.avgPx),
                InitialPrice = GetFromDynamic(item.px),
                CreatedDate = DateTimeHelper.GetDateFromUnix(Convert.ToInt64(item.cTime)),
                UpdatedDate = DateTimeHelper.GetDateFromUnix(Convert.ToInt64(item.uTime)),
                Category = item.category,
                ClientOrderId = item.clOrdId,
                Fee = item.fee,
                FeeCurrency = item.feeCcy,
                Instrument = item.instId,
                InstrumentType = item.instType,
                Leverage = GetFromDynamic(item.lever),
                OrderType = item.ordType,
                OrderSide = item.side,
                PostitionSide = item.posSide,
                State = item.state,
                OKEXTadeMode = item.tdMode,
                OrderIdOKEX = item.ordId
            };
            return order;
        }
        public async Task<OKEXResponse<OrderBook>> GetCurrentOrderBook(string instrument)
        {
            var url = $"/api/v5/market/books?instId=" + instrument + "&sz=" + 5;
            var apiResponse = await CallApi(url, HttpMethod.Get);
            var result = new OKEXResponse<OrderBook>() { Code = apiResponse.Code, Data = null, Message = apiResponse.Message };
            if (apiResponse.Code != 0 || !HasData(apiResponse.Data))
                return result;

            result.Data = new OrderBook();
            result.Data.Asks = new List<OrderBookItem>();
            result.Data.Bids = new List<OrderBookItem>();
            foreach (var v in apiResponse.Data[0].asks)
            {
                result.Data.Asks.Add(new OrderBookItem() { Price = v[0], Amount = v[1] });
            }
            foreach (var v in apiResponse.Data[0].bids)
            {
                result.Data.Bids.Add(new OrderBookItem() { Price = v[0], Amount = v[1] });
            }
            return result;
        }
        public async Task<OKEXResponse<bool>> CancelOrder(string instrument, string cOrderId)
        {
            var resultString = $"/api/v5/trade/cancel-order";
            var body = new
            {
                instId = instrument,
                clOrdId = cOrderId
            };
            var bodyStr = JsonConvert.SerializeObject(body);
            var result = await CallAsyncSign(HttpMethod.Post, resultString, bodyStr);
            dynamic o = JsonConvert.DeserializeObject(result);

            var respone = new OKEXResponse<bool>()
            {
                Code = Convert.ToInt32(o.code),
                Message = Convert.ToString(o.msg),
                Data = false
            };


            if (o.code == 0 || result.Contains("order is already cancelled") || result.Contains("order is already completed") || result.Contains("order does not exist"))
                respone.Data = true;
            else
            {
                respone.Data = false;
                //   _logger.LogWarning("Failed cancel order: " + result);
            }
            return respone;
        }
        /*
        public async Task<bool> AddMarginToIsolatedPosition(string instrument, decimal amount)
        {
            var resultString = $"/api/v5/account/position/margin-balance";
            var body = new
            {
                instId = instrument,
                posSide = "net",
                type = "add",
                amt = amount
            };
            var bodyStr = JsonConvert.SerializeObject(body);
            var result = await CallAsyncSign(HttpMethod.Post, resultString, bodyStr);
            dynamic o = JsonConvert.DeserializeObject(result);
            if (o.code == 0)
                return true;
            return false;
        }
        */


        public async Task<OKEXResponse<bool>> PlaceMarketOrderPosition(string instId, OKEXTadeMode tdMode, string clOrdId, OKEXOrderSide side, string posSide, decimal size, bool reduceOnly)
        {
            var resultString = $"/api/v5/trade/order";
            string ro = "true";
            if (!reduceOnly)
                ro = "false";
            string sz = Convert.ToString(size);
            if (Math.Round(size) == size)
                sz = Convert.ToString(Convert.ToInt64(size));
            var body = new
            {
                instId = instId,
                ordType = "market",
                tdMode = Convert.ToString(tdMode),
                clOrdId = clOrdId,
                side = Convert.ToString(side),
                posSide = posSide,
                sz = sz,
                reduceOnly = ro
            };
            var bodyStr = JsonConvert.SerializeObject(body);
            var result = await CallAsyncSign(HttpMethod.Post, resultString, bodyStr);
            dynamic o = JsonConvert.DeserializeObject(result);
            var respone = new OKEXResponse<bool>() { Code = Convert.ToInt32(o.code), Message = Convert.ToString(o.msg), Data = false };
            if (o.code == 0)
                respone.Data = true;
            else
                respone.Data = false;
            return respone;

        }
        public async Task<OKEXResponse<bool>> PlaceMarketOrder(string instId, OKEXTadeMode tdMode, string clOrdId, OKEXOrderSide side, decimal size, bool reduceOnly)
        {
            var resultString = $"/api/v5/trade/order";
            string ro = "true";
            if (!reduceOnly)
                ro = "false";
            string sz = Convert.ToString(size);
            if (Math.Round(size) == size)
                sz = Convert.ToString(Convert.ToInt64(size));
            var body = new
            {
                instId = instId,
                ordType = "market",
                tdMode = Convert.ToString(tdMode),
                clOrdId = clOrdId,
                side = Convert.ToString(side),
                sz = sz,
                reduceOnly = ro
            };
            var bodyStr = JsonConvert.SerializeObject(body);
            var result = await CallAsyncSign(HttpMethod.Post, resultString, bodyStr);
            dynamic o = JsonConvert.DeserializeObject(result);
            var respone = new OKEXResponse<bool>() { Code = Convert.ToInt32(o.code), Message = Convert.ToString(o.msg), Data = false };
            if (o.code == 0)
                respone.Data = true;
            else
                respone.Data = false;
            return respone;
        }

        public async Task<OKEXResponse<bool>> PlaceLimitOrderPosition(string instId, OKEXTadeMode tdMode, string clOrdId, OKEXOrderSide side, string posSide, decimal size, decimal px, bool reduceOnly)
        {
            var resultString = $"/api/v5/trade/order";
            string ro = "true";
            if (!reduceOnly)
                ro = "false";
            string sz = Convert.ToString(size);
            if (Math.Round(size) == size)
                sz = Convert.ToString(Convert.ToInt64(size));

            var body = new
            {
                instId = instId,
                ordType = "limit",
                px = px.ToString(),
                tdMode = Convert.ToString(tdMode),
                clOrdId = clOrdId,
                side = Convert.ToString(side),
                posSide = posSide,
                sz = sz,
                reduceOnly = ro
            };
            var bodyStr = JsonConvert.SerializeObject(body);
            var result = await CallAsyncSign(HttpMethod.Post, resultString, bodyStr);
            dynamic o = JsonConvert.DeserializeObject(result);
            var respone = new OKEXResponse<bool>() { Code = Convert.ToInt32(o.code), Message = Convert.ToString(o.msg), Data = false };
            if (o.code == 0)
                respone.Data = true;
            else
                respone.Data = false;
            return respone;
        }
        public async Task<OKEXResponse<bool>> PlaceLimitOrder(string instId, OKEXTadeMode tdMode, string clOrdId, OKEXOrderSide side, decimal size, decimal px, bool reduceOnly)
        {
            var resultString = $"/api/v5/trade/order";
            string ro = "true";
            if (!reduceOnly)
                ro = "false";
            string sz = Convert.ToString(size);
            if (Math.Round(size) == size)
                sz = Convert.ToString(Convert.ToInt64(size));
            var body = new
            {
                instId = instId,
                ordType = "limit",
                px = px.ToString(),
                tdMode = Convert.ToString(tdMode),
                clOrdId = clOrdId,
                side = Convert.ToString(side),
                sz = sz,
                reduceOnly = ro//,
                               //      ccy = "LINK"
            };
            var bodyStr = JsonConvert.SerializeObject(body);
            var result = await CallAsyncSign(HttpMethod.Post, resultString, bodyStr);
            dynamic o = JsonConvert.DeserializeObject(result);
            var respone = new OKEXResponse<bool>() { Code = Convert.ToInt32(o.code), Message = Convert.ToString(o.msg), Data = false };
            if (o.code == 0)
                respone.Data = true;
            else
                respone.Data = false;
            return respone;
        }

        private async Task<string> CallAsyncSign(HttpMethod method, string endpoint, string body = null)
        {
            // endpoint = Url + endpoint;
            var request = new HttpRequestMessage(method, endpoint);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.Add("OK-ACCESS-KEY", apiKey);

            var timeStamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
            string sign;
            if (!String.IsNullOrEmpty(body))
            {
                request.Content = new StringContent(body, Encoding.UTF8, "application/json");
                sign = Encryptor.HmacSHA256($"{timeStamp}{method}{endpoint}{body}", apiSecret);
            }
            else
            {
                sign = Encryptor.HmacSHA256($"{timeStamp}{method}{endpoint}", apiSecret);
            }

            request.Headers.Add("OK-ACCESS-SIGN", sign);
            request.Headers.Add("OK-ACCESS-TIMESTAMP", timeStamp.ToString());
            request.Headers.Add("OK-ACCESS-PASSPHRASE", passPhrase);

            var response = await _httpClient.SendAsync(request).ConfigureAwait(false);
            var result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return result;
        }


        public async Task<OKEXResponse<Tuple<DateTime, DateTime>>> GetMaintanaceTime()
        {
            var url = $"/api/v5/system/status";
            var apiResponse = await CallApi(url, HttpMethod.Get);
            var result = new OKEXResponse<Tuple<DateTime, DateTime>>()
            {
                Code = apiResponse.Code,
                Data = null,
                Message = apiResponse.Message
            };
            if (apiResponse.Code != 0)
                return result;

            bool found = false;
            foreach (var v in apiResponse.Data)
            {
                if (v.serviceType != 0)
                    found = true;
            }
            if (!found)
                return result;

            var sDate = DateTimeHelper.GetDateFromUnix(Convert.ToInt64(apiResponse.Data[0].begin));
            var eDate = DateTimeHelper.GetDateFromUnix(Convert.ToInt64(apiResponse.Data[0].end));
            result.Data = new Tuple<DateTime, DateTime>(sDate, eDate);
            return result;

        }
    }
}

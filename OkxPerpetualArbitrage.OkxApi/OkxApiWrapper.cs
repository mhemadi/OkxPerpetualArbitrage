using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OkxPerpetualArbitrage.Application.Contracts.OkxApi;
using OkxPerpetualArbitrage.Application.Models.InfrastructureSettings;
using OkxPerpetualArbitrage.Application.Models.OkexApi;
using OkxPerpetualArbitrage.Application.Models.OkexApi.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OkxPerpetualArbitrage.OkxApi
{


    public class OkxApiWrapper : IOkxApiWrapper
    {
        private readonly OkexApiSetting setting;
        private IOkexApi api;
        private readonly ILogger _logger;
        private int maxTries = 7;
        private int wait = 100;
        //   private readonly IConfiguration _configuration;
        public OkxApiWrapper(IOptions<OkexApiSetting> setting, IOkexApi api, ILogger<OkxApiWrapper> logger)
        {
            this.setting = setting.Value;
            this.api = api;
            wait = this.setting.ApiRetryWaitMiliseconds;
            maxTries = this.setting.ApiMaximumRetries;
            _logger = logger;
        }

        public string GetPerpInstrument(string symbol)
        {
            return symbol + "-USDT-SWAP";
        }
        public string GetSpotInstrument(string symbol)
        {
            return symbol + "-USDT";
        }

        public async Task<List<OKEXBill>> GetRecentFundingBills(int maximumTries = 0, int waitMiliSeconds = 0)
        {
            List<OKEXBill> bills = new List<OKEXBill>();
            int tries = 0;
            if (maximumTries == 0)
                maximumTries = maxTries;
            if (waitMiliSeconds == 0)
                waitMiliSeconds = wait;
            while (maxTries == -1 || tries < maxTries)
            {
                tries++;
                try
                {
                    var result = await api.GetFundingBills();
                    if (result.Data == null)
                    {
                        _logger.LogWarning($"Failed to get funding bills because {result.Message} ");
                        await Task.Delay(waitMiliSeconds);
                        continue;
                    }
                    bills = result.Data;
                    bills = bills.Where(x => x.TimeStamp > DateTime.UtcNow.AddDays(-1)).ToList();
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed getting funding bills");
                    await Task.Delay(waitMiliSeconds);
                }
            }
            return bills;
        }

        public async Task<List<decimal>> GetFundingHistory(string symbol, int days, int maximumTries = 0, int waitMiliSeconds = 0)
        {
            var instrument = GetPerpInstrument(symbol);
            List<decimal> rates = new List<decimal>();
            int tries = 0;
            if (maximumTries == 0)
                maximumTries = maxTries;
            if (waitMiliSeconds == 0)
                waitMiliSeconds = wait;
            while (maxTries == -1 || tries < maxTries)
            {
                tries++;
                try
                {
                    var result = await api.GetFundingIncomeHistory(instrument);
                    if (result.Data == null)
                    {
                        _logger.LogWarning($"Failed to get funding history for {symbol} because {result.Message} ");
                        await Task.Delay(waitMiliSeconds);
                        continue;
                    }
                    rates = result.Data;
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed getting funding bills");
                    await Task.Delay(waitMiliSeconds);
                }
            }
            int c = days * 3;
            if (rates.Count < c + 1)
                return rates;
            return rates.GetRange(0, c);
        }

        public async Task<string> PlaceOrder(string instId, OKEXOrderType orderType, OKEXTadeMode tdMode, OKEXOrderSide side, OKEXPostitionSide posSide, decimal size, decimal px, bool reduceOnly, int maximumTries = 0, int waitMiliSeconds = 0)
        {
            int tries = 0;
            if (maximumTries == 0)
                maximumTries = maxTries;
            if (waitMiliSeconds == 0)
                waitMiliSeconds = wait;
            OKEXResponse<bool> ok = new OKEXResponse<bool>() { Code = 1, Data = false, Message = "" };
            string orderId = DateTime.UtcNow.Year.ToString();
            Random r = new Random();
            orderId += r.Next(100000000, 1000000000).ToString();
            while (maximumTries == -1 || tries < maximumTries)
            {
                tries++;
                try
                {
                    if (orderType == OKEXOrderType.limit)
                    {
                        if (posSide == OKEXPostitionSide.NotPosition)
                            ok = await api.PlaceLimitOrder(instId, tdMode, orderId, side, size, px, false);
                        else
                            ok = await api.PlaceLimitOrderPosition(instId, tdMode, orderId, side, Convert.ToString(posSide).ToLower(), size, px, reduceOnly);
                    }
                    if (orderType == OKEXOrderType.market)
                    {
                        if (posSide == OKEXPostitionSide.NotPosition)
                            ok = await api.PlaceMarketOrder(instId, tdMode, orderId, side, size, false);
                        else
                            ok = await api.PlaceMarketOrderPosition(instId, tdMode, orderId, side, Convert.ToString(posSide).ToLower(), size, reduceOnly);
                    }
                    if (ok.Data)
                        break;
                    else
                    {
                        _logger.LogWarning($"Failed placing {orderType} order for instrument {instId} {side} size {size} price {px} error {ok.Message}");
                        await Task.Delay(waitMiliSeconds);
                    }

                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed placing order");
                    await Task.Delay(waitMiliSeconds);
                }

            }
            if (!ok.Data)
                orderId = "-1";
            return orderId;
        }
        public async Task<OKEXOrder> GetOrder(string instrument, string orderId, int maximumTries = 0, int waitMiliSeconds = 0)
        {
            OKEXOrder order = null;
            int tries = 0;
            if (maximumTries == 0)
                maximumTries = maxTries;
            if (waitMiliSeconds == 0)
                waitMiliSeconds = wait;
            while (maximumTries == -1 || tries < maximumTries)
            {
                tries++;
                try
                {
                    var result = await api.GetOrderByClientOrderId(instrument, orderId);
                    order = result.Data;
                    if (order == null)
                    {
                        _logger.LogWarning($"Failed to get {instrument} order with id {orderId} because {result.Message} ");
                        await Task.Delay(waitMiliSeconds);
                    }
                    else
                        break;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed getting order");
                    await Task.Delay(waitMiliSeconds);
                }
            }
            return order;
        }
        public async Task<bool> CancelOrder(string instrument, string orderId, int maximumTries = 0, int waitMiliSeconds = 0)
        {
            bool ok = false;
            int tries = 0;
            if (maximumTries == 0)
                maximumTries = maxTries;
            if (waitMiliSeconds == 0)
                waitMiliSeconds = wait;
            while (maximumTries == -1 || tries < maximumTries)
            {
                tries++;
                try
                {
                    var result = await api.CancelOrder(instrument, orderId);
                    ok = result.Data;
                    if (!ok)
                    {
                        _logger.LogWarning($"Failed to cancel {instrument} order with id {orderId} because {result.Message} ");
                        await Task.Delay(waitMiliSeconds);
                    }
                    else
                        break;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed cancelling order");
                    await Task.Delay(waitMiliSeconds);
                }
            }
            return ok;
        }

        public async Task<OrderBook> GetOrderBook(string instrument, int maximumTries = 0, int waitMiliSeconds = 0)
        {
            OrderBook ob = null;
            int tries = 0;
            if (maximumTries == 0)
                maximumTries = maxTries;
            if (waitMiliSeconds == 0)
                waitMiliSeconds = wait;
            while (maximumTries == -1 || tries < maximumTries)
            {
                tries++;
                try
                {
                    var result = await api.GetCurrentOrderBook(instrument);
                    ob = result.Data;
                    if (ob == null)
                    {
                        _logger.LogWarning($"Failed to get {instrument} orderbood because {result.Message} ");
                        await Task.Delay(waitMiliSeconds);
                    }
                    else
                        break;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed getiing orderbook");
                    await Task.Delay(waitMiliSeconds);
                }
            }
            return ob;
        }

        public async Task<Tuple<decimal, decimal, decimal, decimal>> GetSpotAndPerpMakerAndTakerFees(string symbol, int maximumTries = 0, int waitMiliSeconds = 0)
        {
            Tuple<decimal, decimal, decimal, decimal> t = null;
            int tries = 0;
            if (maximumTries == 0)
                maximumTries = maxTries;
            if (waitMiliSeconds == 0)
                waitMiliSeconds = wait;
            while (maximumTries == -1 || tries < maximumTries)
            {
                tries++;
                try
                {
                    var spotFee = await api.GetFeeForMakerAndTaker(OKEXInstrumentType.SPOT, GetSpotInstrument(symbol));
                    if (spotFee.Data == null)
                    {
                        _logger.LogWarning($"Failed to get {symbol} spot fee because {spotFee.Message} ");
                        await Task.Delay(waitMiliSeconds);
                        continue;
                    }
                    var perpFee = await api.GetFeeForMakerAndTaker(OKEXInstrumentType.SWAP, GetPerpInstrument(symbol));
                    if (perpFee.Data == null)
                    {
                        _logger.LogWarning($"Failed to get {symbol} spot fee because {perpFee.Message} ");
                        await Task.Delay(waitMiliSeconds);
                        continue;
                    }
                    t = new Tuple<decimal, decimal, decimal, decimal>(spotFee.Data.Item1, spotFee.Data.Item2, perpFee.Data.Item1, perpFee.Data.Item2);
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed getting fees");
                    await Task.Delay(waitMiliSeconds);
                }
            }
            return t;
        }

        public async Task<Tuple<decimal, decimal, decimal>> GetPerpContractData(string symbol, int maximumTries = 0, int waitMiliSeconds = 0)
        {
            Tuple<decimal, decimal, decimal> t = null;
            int tries = 0;
            if (maximumTries == 0)
                maximumTries = maxTries;
            if (waitMiliSeconds == 0)
                waitMiliSeconds = wait;
            while (maximumTries == -1 || tries < maximumTries)
            {
                tries++;
                try
                {
                    var result = await api.GetPerpContractMinSizeAndTickSizeAndContractValue(GetPerpInstrument(symbol));
                    t = result.Data;
                    if (t == null)
                    {
                        _logger.LogWarning($"Failed to get {symbol} perp contract data because {result.Message} ");
                        await Task.Delay(waitMiliSeconds);
                    }
                    else
                        break;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed getting perp contract data");
                    await Task.Delay(waitMiliSeconds);
                }
            }
            return t;
        }
        public async Task<Tuple<decimal, decimal, decimal>> GetSpotContractData(string symbol, int maximumTries = 0, int waitMiliSeconds = 0)
        {
            Tuple<decimal, decimal, decimal> t = null;
            int tries = 0;
            if (maximumTries == 0)
                maximumTries = maxTries;
            if (waitMiliSeconds == 0)
                waitMiliSeconds = wait;
            while (maximumTries == -1 || tries < maximumTries)
            {
                tries++;
                try
                {
                    var result = await api.GetSpotMinSizeAndLotSizeAndTickSize(GetSpotInstrument(symbol));
                    t = result.Data;
                    if (t == null)
                    {
                        _logger.LogWarning($"Failed to get {symbol} spot contract data because {result.Message} ");
                        await Task.Delay(waitMiliSeconds);
                    }
                    else
                        break;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed getting spot contract data");
                    await Task.Delay(waitMiliSeconds);
                }
            }
            return t;
        }


        public async Task<Tuple<decimal, decimal>> GetFundingRates(string symbol, int maximumTries = 0, int waitMiliSeconds = 0)
        {
            Tuple<decimal, decimal> t = null;
            int tries = 0;
            if (maximumTries == 0)
                maximumTries = maxTries;
            if (waitMiliSeconds == 0)
                waitMiliSeconds = wait;
            while (maximumTries == -1 || tries < maximumTries)
            {
                tries++;
                try
                {

                    var result = await api.GetCurrentAndNextFundingRatesAndFundingTime(GetPerpInstrument(symbol));
                    if (result.Data == null)
                    {
                        _logger.LogWarning($"Failed to get {symbol} dunding rates because {result.Message} ");
                        await Task.Delay(waitMiliSeconds);
                    }
                    else
                    {
                        t = new Tuple<decimal, decimal>(result.Data.Item1, result.Data.Item2);
                        break;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed getting funding rates");
                    await Task.Delay(waitMiliSeconds);
                }
            }
            return t;
        }


        public async Task<List<OKEXMarketInfo>> GetAllSymbols(int maximumTries = 0, int waitMiliSeconds = 0)
        {
            //We could maybe use funding rates returned by this api and not get them later

            List<OKEXMarketInfo> spots = null;
            List<OKEXMarketInfo> perps = null;
            int tries = 0;
            if (maximumTries == 0)
                maximumTries = maxTries;
            if (waitMiliSeconds == 0)
                waitMiliSeconds = wait;


            while (maximumTries == -1 || tries < maximumTries)
            {
                tries++;
                try
                {
                    var spotResult = await api.GetInstruments(OKEXInstrumentType.SPOT);
                    if (spotResult.Data == null)
                    {
                        _logger.LogWarning($"Failed to get spot symbolse because {spotResult.Message} ");
                        await Task.Delay(waitMiliSeconds);
                        continue;
                    }
                    await Task.Delay(wait);
                    var perpResult = await api.GetInstruments(OKEXInstrumentType.SWAP);
                    if (perpResult.Data == null)
                    {
                        _logger.LogWarning($"Failed to get spot symbolse because {perpResult.Message} ");
                        await Task.Delay(waitMiliSeconds);
                        continue;
                    }
                    spots = spotResult.Data;
                    perps = perpResult.Data;
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed getting all instruments data");
                    await Task.Delay(waitMiliSeconds);
                }
            }
            List<OKEXMarketInfo> symbols = new List<OKEXMarketInfo>();
            foreach (var perp in perps)
            {
                if (!perp.Instrument.Contains("USDT"))
                    continue;
                string spotInstrument = perp.Instrument.Replace("-SWAP", "");
                string symbol = spotInstrument.Replace("-USDT", "");
                var spot = spots.Where(x => x.Instrument == spotInstrument).FirstOrDefault();
                if (spot != null)
                    symbols.Add(new OKEXMarketInfo() { SpotAsk = spot.SpotAsk, PerpBid = perp.SpotBid, Instrument = symbol, SpotBid = spot.SpotBid, PerpAsk = perp.SpotAsk });
            }
            return symbols;
        }
        public async Task<OKEXBalance> GetUSDTBalance(int maximumTries = 0, int waitMiliSeconds = 0)
        {
            return await GetBalance("USDT", maximumTries, waitMiliSeconds);
        }

        public async Task<OKEXBalance> GetBalance(string symbol, int maximumTries = 0, int waitMiliSeconds = 0)
        {
            OKEXBalance ob = null;

            int tries = 0;
            if (maximumTries == 0)
                maximumTries = maxTries;
            if (waitMiliSeconds == 0)
                waitMiliSeconds = wait;

            while (maxTries == -1 || tries < maxTries)
            {
                tries++;
                try
                {
                    var balances = await api.GetBalances();
                    if (balances.Data == null)
                    {
                        _logger.LogWarning($"Failed to get {symbol} balance because {balances.Message} ");
                        await Task.Delay(waitMiliSeconds);
                        continue;
                    }
                    ob = balances.Data.Where(x => x.Symbol == symbol).FirstOrDefault();
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed getting " + symbol + " balance");
                    await Task.Delay(waitMiliSeconds);
                }
            }
            return ob;
        }

        public async Task<decimal> GetPositionSize(string symbol, int maximumTries = 0, int waitMiliSeconds = 0)
        {
            decimal size = 0;
            int tries = 0;
            if (maximumTries == 0)
                maximumTries = maxTries;
            if (waitMiliSeconds == 0)
                waitMiliSeconds = wait;
            while (maxTries == -1 || tries < maxTries)
            {
                tries++;
                try
                {
                    var result = await api.GetPositionSize(OKEXInstrumentType.SWAP, GetPerpInstrument(symbol));
                    size = result.Data;
                    if (size == null)
                    {
                        _logger.LogWarning($"Failed to get {symbol} position size because {result.Message} ");
                        await Task.Delay(waitMiliSeconds);
                    }
                    else
                        break;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed getting " + symbol + " PositionSize");
                    await Task.Delay(waitMiliSeconds);
                }
            }
            return size;
        }

        public async Task<int> GetMinutesToMaintenace(int maximumTries = 0, int waitMiliSeconds = 0)
        {
            Tuple<DateTime, DateTime> times = null;
            int tries = 0;
            if (maximumTries == 0)
                maximumTries = maxTries;
            if (waitMiliSeconds == 0)
                waitMiliSeconds = wait;
            while (maxTries == -1 || tries < maxTries)
            {
                tries++;
                try
                {
                    var result = await api.GetMaintanaceTime();
                    times = result.Data;
                    if (times == null)
                    {
                        _logger.LogWarning($"Failed to get maintenance time because {result.Message} ");
                        await Task.Delay(waitMiliSeconds);
                    }
                    else
                        break;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed getting maintenance time");
                    await Task.Delay(waitMiliSeconds);
                }
            }
            if (times == null)
                return -1;
            if (times.Item1 <= DateTime.UtcNow)
                return 0;
            var d = Math.Ceiling((times.Item1 - DateTime.UtcNow).TotalMinutes);
            return Convert.ToInt32(d);
        }
    }
}

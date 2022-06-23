using Microsoft.Extensions.Logging;
using OkxPerpetualArbitrage.Application.Contracts.ApiService;
using OkxPerpetualArbitrage.Application.Contracts.Logic;
using OkxPerpetualArbitrage.Application.Models.OkexApi;
using OkxPerpetualArbitrage.Application.Models.OkexApi.Enums;
using OkxPerpetualArbitrage.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OkxPerpetualArbitrage.Application.Services
{


    public class OrderCreateLogic : IOrderCreateLogic
    {
        private readonly IApiService _apiService;
        private readonly ILogger<OrderCreateLogic> _logger;

        public OrderCreateLogic(IApiService apiService, ILogger<OrderCreateLogic> logger)
        {
            _apiService = apiService;
            _logger = logger;
        }
        public async Task<string> CreatePerpOrder(string symbol, decimal spreadThreshold, decimal size, PotentialPosition pp, bool tryToBeMaker, bool open)
        {
            string perpInstrumnet = _apiService.GetPerpInstrument(symbol);
            string spotInstrumnet = _apiService.GetSpotInstrument(symbol);
            OrderBook spotOrderBook = new OrderBook();
            OrderBook perpOrderBook = new OrderBook();
            for (int i = 0; i < 3; i++)
            {
                await Task.Delay(1 * 1000);
                spotOrderBook = await _apiService.GetOrderBook(spotInstrumnet, 3, 100);
                perpOrderBook = await _apiService.GetOrderBook(perpInstrumnet, 3, 100);
                if (perpOrderBook == null || spotOrderBook == null)
                {
                    _logger.LogError("Failed to get order books to create perp order for {symbol}", symbol);
                    return "-1";
                }
                var spread = (perpOrderBook.Bids[0].Price - spotOrderBook.Asks[0].Price) / ((perpOrderBook.Bids[0].Price + spotOrderBook.Asks[0].Price) / 2) * 100;
                if (!open)
                    spread = (spotOrderBook.Bids[0].Price - perpOrderBook.Asks[0].Price) / ((perpOrderBook.Asks[0].Price + spotOrderBook.Bids[0].Price) / 2) * 100;
                if (spread < spreadThreshold)
                    return "-1";
                _logger.LogInformation("{symbol} spread is good at {spread} for try {try} with perp bid {pb} perp ask {pa} spot bid {sb} spot ask {sa}", symbol, spread, i + 1, perpOrderBook.Bids[0].Price, perpOrderBook.Asks[0].Price, spotOrderBook.Bids[0].Price, spotOrderBook.Asks[0].Price);
            }
            var bottomPerp = perpOrderBook.Bids[0].Price;
            var topPerp = perpOrderBook.Asks[0].Price;
            decimal toBeAddedToBot = ((topPerp - bottomPerp) / 3);
            if (toBeAddedToBot < pp.TickSizePerp)
                toBeAddedToBot = pp.TickSizePerp;
            toBeAddedToBot -= Decimal.Remainder(toBeAddedToBot, pp.TickSizePerp);
            if (!tryToBeMaker)
                toBeAddedToBot = 0;
            decimal perpPrice = bottomPerp + (toBeAddedToBot);
            if (!open)
                perpPrice = topPerp - (toBeAddedToBot);
            OKEXOrderSide side = OKEXOrderSide.sell;
            if (!open)
                side = OKEXOrderSide.buy;
            return await _apiService.PlaceOrder(perpInstrumnet, OKEXOrderType.limit, OKEXTadeMode.cross, side, OKEXPostitionSide.SHORT, size, perpPrice, !open, 10, 100);
        }
        public async Task<Tuple<string, decimal>> CreateFirstSpotOrder(string symbol, decimal perpOrderSize, PotentialPosition pp, bool tryToBeMaker, bool open)
        {
            string spotInstrumnet = _apiService.GetSpotInstrument(symbol);
            var spotOrderBook = await _apiService.GetOrderBook(spotInstrumnet, 100, 500);
            if (spotOrderBook == null)
            {
                _logger.LogError("Failed to get order books to create order for {symbol}", symbol);
                return new Tuple<string, decimal>("-1", 0);
            }
            var bottomSpot = spotOrderBook.Bids[0].Price;
            var topSpot = spotOrderBook.Asks[0].Price;
            decimal toBeSubtractedFromTop = ((topSpot - bottomSpot) / 3);
            if (toBeSubtractedFromTop < pp.TickSizeSpot)
                toBeSubtractedFromTop = pp.TickSizeSpot;
            toBeSubtractedFromTop -= Decimal.Remainder(toBeSubtractedFromTop, pp.TickSizeSpot);
            if (!tryToBeMaker)
                toBeSubtractedFromTop = 0;
            decimal spotPrice = topSpot - toBeSubtractedFromTop;
            if (!open)
                spotPrice = bottomSpot + toBeSubtractedFromTop;
            var spotSize = (perpOrderSize * pp.ContractValuePerp);
            if (open)
            {
                decimal feeToBeAdded = perpOrderSize * pp.ContractValuePerp * pp.FeeSpotTaker * -1.1m;
                if (feeToBeAdded < pp.LotSizeSpot)
                    feeToBeAdded = pp.LotSizeSpot;
                decimal tmpRemainder = Decimal.Remainder(feeToBeAdded, pp.LotSizeSpot);
                if (tmpRemainder != 0)
                    feeToBeAdded = feeToBeAdded - tmpRemainder + pp.LotSizeSpot;
                spotSize = (perpOrderSize * pp.ContractValuePerp) + feeToBeAdded;
            }
            OKEXOrderSide side = OKEXOrderSide.buy;
            if (!open)
                side = OKEXOrderSide.sell;

            var orderId = await _apiService.PlaceOrder(spotInstrumnet, OKEXOrderType.limit, OKEXTadeMode.cross, side, OKEXPostitionSide.NotPosition, spotSize, spotPrice, false, 500, 500);
            return new Tuple<string, decimal>(orderId, spotSize);
        }
        public async Task<string> CreateAdditionalSpotOrder(string symbol, PotentialPosition pp, decimal spotSize, bool tryToBeMaker, bool open)
        {
            string spotInstrumnet = _apiService.GetSpotInstrument(symbol);
            var spotOrderBook = await _apiService.GetOrderBook(spotInstrumnet, 100, 500);
            if (spotOrderBook == null)
            {
                _logger.LogError("Failed to get order books to create order for {symbol}", symbol);
                return "-1";
            }
            var bottomSpot = spotOrderBook.Bids[0].Price;
            var topSpot = spotOrderBook.Asks[0].Price;
            var toBeSubtractedFromTop = ((topSpot - bottomSpot) / 3);
            if (toBeSubtractedFromTop < pp.TickSizeSpot)
                toBeSubtractedFromTop = pp.TickSizeSpot;
            toBeSubtractedFromTop -= Decimal.Remainder(toBeSubtractedFromTop, pp.TickSizeSpot);
            if (!tryToBeMaker)
                toBeSubtractedFromTop = 0;
            var spotPrice = topSpot - toBeSubtractedFromTop;
            if (!open)
                spotPrice = bottomSpot + toBeSubtractedFromTop;
            OKEXOrderSide side = OKEXOrderSide.buy;
            if (!open)
                side = OKEXOrderSide.sell;
            return await _apiService.PlaceOrder(spotInstrumnet, OKEXOrderType.limit, OKEXTadeMode.cross, side, OKEXPostitionSide.NotPosition, spotSize, spotPrice, false, 500, 500);

        }
    }
}

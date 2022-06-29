using Microsoft.Extensions.Logging;
using OkxPerpetualArbitrage.Application.Contracts.OkxApi;
using OkxPerpetualArbitrage.Application.Contracts.Logic;
using OkxPerpetualArbitrage.Application.Models.OkexApi;
using OkxPerpetualArbitrage.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OkxPerpetualArbitrage.Application.Services
{

    public class OrderStateCheckLogic : IOrderStateCheckLogic
    {
        private readonly IOkxApiWrapper _apiService;
        private readonly ILogger<OrderStateCheckLogic> _logger;

        public OrderStateCheckLogic(IOkxApiWrapper apiService, ILogger<OrderStateCheckLogic> logger)
        {
            _apiService = apiService;
            _logger = logger;
        }
        public async Task<bool> CanKeepOrderPerp(string symbol, OKEXOrder perpOrder, decimal spreadThreshold, bool open)
        {
            string perpInstrumnet = _apiService.GetPerpInstrument(symbol);
            string spotInstrumnet = _apiService.GetSpotInstrument(symbol);
            var spotOrderBook = await _apiService.GetOrderBook(spotInstrumnet, 20, 100);
            var perpOrderBook = await _apiService.GetOrderBook(perpInstrumnet, 20, 100);
            if (perpOrderBook == null || spotOrderBook == null)
            {
                _logger.LogError("Failed to get order books to check order status for {symbol}", symbol);
                return true;
            }
            var bottomPerp = perpOrderBook.Bids[0].Price;
            var topPerp = perpOrderBook.Asks[0].Price;
            bool isInMid = (perpOrder.InitialPrice >= bottomPerp && perpOrder.InitialPrice <= topPerp);
            var spread = (perpOrderBook.Bids[0].Price - spotOrderBook.Asks[0].Price) / ((perpOrderBook.Bids[0].Price + spotOrderBook.Asks[0].Price) / 2) * 100;
            if (!open)
                spread = (spotOrderBook.Bids[0].Price - perpOrderBook.Asks[0].Price) / ((perpOrderBook.Asks[0].Price + spotOrderBook.Bids[0].Price) / 2) * 100;
            if (isInMid && spread >= spreadThreshold)
            {
                _logger.LogInformation("{symbol} spread is good to keep order {orderId} at {spread}  with perp bid {pb} perp ask {pa} spot bid {sb} spot ask {sa}", symbol, perpOrder.ClientOrderId, spread, perpOrderBook.Bids[0].Price, perpOrderBook.Asks[0].Price, spotOrderBook.Bids[0].Price, spotOrderBook.Asks[0].Price);
                return true;
            }

            return false;
        }
        public async Task<bool> IsSpotBelowMid(string symbol, OKEXOrder spotOrder, PotentialPosition pp)
        {
            string spotInstrumnet = _apiService.GetSpotInstrument(symbol);
            var spotOrderBook = await _apiService.GetOrderBook(spotInstrumnet, 20, 500);
            if (spotOrderBook == null)
            {
                _logger.LogError("Failed to get spot order books to check order for {symbol}", symbol);
                return false;
            }
            var midPrice = (spotOrderBook.Asks[0].Price + spotOrderBook.Bids[0].Price) / 2m;
            //change
            if (spotOrder.InitialPrice == spotOrderBook.Asks[0].Price && spotOrder.InitialSize >= spotOrderBook.Asks[0].Amount)
            {
                midPrice = (spotOrderBook.Asks[1].Price + spotOrderBook.Bids[0].Price) / 2m;
            }
            bool isBelowMid = spotOrder.InitialPrice < midPrice;
            if (spotOrderBook.Asks[0].Price - spotOrderBook.Bids[0].Price < 4 * pp.TickSizeSpot)
                isBelowMid = spotOrder.InitialPrice >= spotOrderBook.Bids[0].Price && spotOrder.InitialPrice <= spotOrderBook.Asks[0].Price;
            return isBelowMid;
        }

        public async Task<bool> IsSpotAboveMid(string symbol, OKEXOrder spotOrder, PotentialPosition pp)
        {
            string spotInstrumnet = _apiService.GetSpotInstrument(symbol);
            var spotOrderBook = await _apiService.GetOrderBook(spotInstrumnet, 20, 500);
            if (spotOrderBook == null)
            {
                _logger.LogError("Failed to get spot order books to check order for {symbol}", symbol);
                return false;
            }
            var midPrice = (spotOrderBook.Asks[0].Price + spotOrderBook.Bids[0].Price) / 2m;
            if (spotOrder.InitialPrice == spotOrderBook.Bids[0].Price && spotOrder.InitialSize >= spotOrderBook.Bids[0].Amount)
            {
                midPrice = (spotOrderBook.Asks[0].Price + spotOrderBook.Bids[1].Price) / 2m;
            }
            bool isAboveMid = spotOrder.InitialPrice > midPrice;
            if (spotOrderBook.Asks[0].Price - spotOrderBook.Bids[0].Price < 4 * pp.TickSizeSpot)
                isAboveMid = spotOrder.InitialPrice >= spotOrderBook.Bids[0].Price && spotOrder.InitialPrice <= spotOrderBook.Asks[0].Price;
            return isAboveMid;
        }
    }
}

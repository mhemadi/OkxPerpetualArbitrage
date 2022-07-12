using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OkxPerpetualArbitrage.Application.Contracts.Logic;
using OkxPerpetualArbitrage.Application.Contracts.OkxApi;
using OkxPerpetualArbitrage.Application.Contracts.Persistance;
using OkxPerpetualArbitrage.Application.Models.InfrastructureSettings;
using OkxPerpetualArbitrage.Application.Models.OkexApi.Enums;
using OkxPerpetualArbitrage.Domain.Entities;
using OkxPerpetualArbitrage.Domain.Entities.Enums;

namespace OkxPerpetualArbitrage.Application.Services
{
    public class PositionCloseLogic : IPositionCloseLogic
    {
        private readonly IPositionDemandRepository _positionDemandRepository;
        private readonly IOrderFillCreateLogic _orderFillCreateLogic;
        private readonly GeneralSetting _setting;
        private readonly ILogger<PositionCloseLogic> _logger;
        private readonly IOkxApiWrapper _apiWrapper;
        private readonly IFundingIncomeRepository _fundingIncomeRepository;
        private readonly IPositionHistoryRepository _positionHistoryRepository;
        private readonly IPositionChunkCreateLogic _positionChunkCreateLogic;
        private readonly IOrderFillRepository _orderFillRepository;
        private readonly IPotentialPositionRepository _potentialPositionRepository;
        private readonly IPositionCheckLogic _positionCheckLogic;

        public PositionCloseLogic(IPositionDemandRepository positionDemandRepository, IOrderFillCreateLogic orderFillCreateLogic, IOptions<GeneralSetting> setting
            , ILogger<PositionCloseLogic> logger, IOkxApiWrapper apiWrapper, IFundingIncomeRepository fundingIncomeRepository, IPositionHistoryRepository positionHistoryRepository
            , IPositionChunkCreateLogic positionChunkCreateLogic, IOrderFillRepository orderFillRepository, IPotentialPositionRepository potentialPositionRepository
            , IPositionCheckLogic positionCheckLogic)
        {
            _positionDemandRepository = positionDemandRepository;
            _orderFillCreateLogic = orderFillCreateLogic;
            _setting = setting.Value;
            _logger = logger;
            _apiWrapper = apiWrapper;
            _fundingIncomeRepository = fundingIncomeRepository;
            _positionHistoryRepository = positionHistoryRepository;
            _positionChunkCreateLogic = positionChunkCreateLogic;
            _orderFillRepository = orderFillRepository;
            _potentialPositionRepository = potentialPositionRepository;
            _positionCheckLogic = positionCheckLogic;
        }

        public async Task Close(string symbol, int positionDemandId, decimal lotSize, decimal lotSizeChunk, decimal minSpread, PotentialPosition potentialPosition, bool isInstant)
        {
            var demand = await _positionDemandRepository.GetPositionDemandNoTracking(positionDemandId);
            if (demand == null)
            {
                _logger.LogError("Could not find the demand to close the position {symbol} {positionDemandId} ", symbol, positionDemandId);
                throw new Exception("Could not find the demand to close the position");
            }
            try
            {
                int tries = 0;
                while (lotSize != 0 && !demand.IsCanceled && tries < _setting.MaxCloseTries)
                {
                    decimal filled;
                    _logger.LogInformation("startining try {tries} to close chunksize {lotSizeChunk} for demand {positionDemandId}", tries, lotSizeChunk, positionDemandId);
                    if (isInstant)
                        filled = await ClosePositionInstant(symbol, lotSize, positionDemandId, potentialPosition);
                    else
                        filled = await _positionChunkCreateLogic.OpenClosePositionChunck(symbol, lotSizeChunk, positionDemandId, minSpread, potentialPosition, _setting.TryToBeMaker, false);

                    _logger.LogInformation("filled {filled} for demand {reqId}", filled, positionDemandId);
                    tries++;
                    lotSize -= filled;
                    if (lotSizeChunk > lotSize)
                        lotSizeChunk = lotSize;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed closing position {symbol} {positionDemandId} ", symbol, positionDemandId);
            }
            finally
            {
                _logger.LogInformation("Updating the initial close demand {positionDemandId}", positionDemandId);

                demand = await _positionDemandRepository.GetPositionDemandNoTracking(positionDemandId);
                var demandTracked = await _positionDemandRepository.GetByIdAsync(positionDemandId);
                demandTracked.Filled = demand.Filled;
                demandTracked.IsCanceled = demand.IsCanceled;
                demandTracked.PositionDemandState = PositionDemandState.InPosition;
                if (demandTracked.Filled == 0)
                    demandTracked.PositionDemandState = PositionDemandState.Done;

                var pp = await _potentialPositionRepository.GetPotentialPosition(symbol);
                var fills = await _orderFillRepository.GetOrderFillsByPositionDemandId(positionDemandId);
                decimal spotSize = fills.Where(x => x.PartInPosition == PartInPosition.SpotSell).Sum(x => x.Filled);
                decimal size = fills.Where(x => x.PartInPosition == PartInPosition.PerpCloseSell).Sum(x => x.Filled);
                size *= pp.ContractValuePerp;
                if (demandTracked.Filled != 0)
                {
                    decimal sellPriceSpot = 0;
                    decimal buyPricePerp = 0;
                    foreach (var v in fills.Where(x => x.PartInPosition == PartInPosition.SpotSell))
                        sellPriceSpot += (v.Price * (v.Filled));

                    foreach (var v in fills.Where(x => x.PartInPosition == PartInPosition.PerpCloseSell))
                        buyPricePerp += (v.Price * v.Filled * pp.ContractValuePerp);
                    if (spotSize > 0 && size > 0)
                    {
                        sellPriceSpot /= spotSize;
                        buyPricePerp /= size;
                        var spread = (sellPriceSpot - buyPricePerp) / ((buyPricePerp + sellPriceSpot) / 2) * 100;
                        demandTracked.ActualSpread = spread;
                    }
                    else
                        _logger.LogError("Demand {demandId} has filled at {filled} but spot size {spotSize} and perp size {perpsize}", positionDemandId, demandTracked.Filled, spotSize, size);
                }

                await UpdateRequestAfterClose(demandTracked, pp);
                demand = await _positionDemandRepository.GetPositionDemandNoTracking(positionDemandId);

                decimal spotSizeAvailable = fills.Where(x => x.PartInPosition == PartInPosition.SpotSell).Sum(x => x.Filled) + fills.Where(x => x.PartInPosition == PartInPosition.SpotBuy).Sum(x => x.Fee);
                decimal sizeClose = fills.Where(x => x.PartInPosition == PartInPosition.PerpCloseSell).Sum(x => x.Filled);
                sizeClose *= pp.ContractValuePerp;

                if (sizeClose != demand.Filled)
                {
                    _logger.LogCritical("sum of order fill size {sumSize} is not equal to demand fill size {filled}", sizeClose, demand.Filled);
                    demandTracked.Filled = sizeClose;
                    await _positionDemandRepository.UpdateAsync(demandTracked);
                }
                if (spotSizeAvailable < sizeClose)
                {
                    _logger.LogCritical("sum of order fill size {sumSize} is larger than spot size {spotSize}", sizeClose, spotSizeAvailable);
                }

                await _positionCheckLogic.Checkposition(symbol, pp);
            }
        }


        public async Task<decimal> ClosePositionInstant(string symbol, decimal lotSize, int positionDemandId, PotentialPosition potentialPosition)
        {
            decimal fillLotSize = 0;
            var demand = await _positionDemandRepository.GetPositionDemandNoTracking(positionDemandId);
            if (demand == null)
            {
                _logger.LogError("Could not find the demand to close the position instantly {symbol} {positionDemandId} ", symbol, positionDemandId);
                throw new Exception("Could not find the demand to close the position instantly");
            }
            string perpInstrumnet = _apiWrapper.GetPerpInstrument(symbol);
            string spotInstrumnet = _apiWrapper.GetSpotInstrument(symbol);
            _apiWrapper.SetWait(500);
            _apiWrapper.SetMaxTry(10);
            var perpOrderId = await _apiWrapper.PlaceOrder(perpInstrumnet, OKEXOrderType.market, OKEXTadeMode.cross, OKEXOrderSide.buy, OKEXPostitionSide.SHORT, lotSize, 0, true);
            if (perpOrderId == "-1")
            {
                _logger.LogError("Failed to place first close position order for {symbol} and {lotSize}", symbol, lotSize);
                return 0;
            }

            var spotOrderId = await _apiWrapper.PlaceOrder(spotInstrumnet, OKEXOrderType.market, OKEXTadeMode.cross, OKEXOrderSide.sell, OKEXPostitionSide.NotPosition, lotSize * potentialPosition.ContractValuePerp, 0, true);
            if (spotOrderId == "-1")
            {
                _logger.LogCritical("Failed to place second close position order for {symbol} and {lotSize}", symbol, lotSize);
                return 0;
            }
            await Task.Delay(100);
            var perpOrder = await _apiWrapper.GetOrder(perpInstrumnet, perpOrderId);
            if (perpOrder == null)
            {
                _logger.LogCritical("Failed to retreive first close position order for {symbol} and {lotSize}", symbol, lotSize);
                return 0;
            }
            await _orderFillCreateLogic.AddOrderFill(perpOrder, demand.PositionDemandId, PartInPosition.PerpCloseSell, potentialPosition);
            fillLotSize = perpOrder.Size;
            var spotpOrder = await _apiWrapper.GetOrder(spotInstrumnet, spotOrderId);
            if (spotpOrder == null)
            {
                _logger.LogCritical("Failed to retreive second close position order for {symbol} and {lotSize}", symbol, lotSize);
                return fillLotSize;
            }
            await _orderFillCreateLogic.AddOrderFill(spotpOrder, demand.PositionDemandId, PartInPosition.SpotSell, potentialPosition);
            return fillLotSize;
        }

        private async Task UpdateRequestAfterClose(PositionDemand currentDemand, PotentialPosition pp)
        {
            var notDoneDemands = await _positionDemandRepository.GetIncompleteDemandsNoTracking(currentDemand.Symbol);
            decimal totalBuySpot = 0;
            decimal totalSellSpot = 0;
            decimal totalBuyPerp = 0;
            decimal totalSellPerp = 0;
            decimal totalFeeUsdt = 0;
            decimal totalFunding = 0;
            decimal spotBuyPrice = 0;
            decimal spotSellPrice = 0;
            decimal perpBuyPrice = 0;
            decimal perpSellPrice = 0;

            DateTime? openDate = null;

            foreach (var demand in notDoneDemands)
            {
                if (demand.PositionDemandState == PositionDemandState.InProgress && demand.PositionDemandId != currentDemand.PositionDemandId)
                {
                    currentDemand.UpdateDate = DateTime.UtcNow;
                    await _positionDemandRepository.UpdateAsync(currentDemand);
                    return;
                }
                foreach (var fill in await _orderFillRepository.GetOrderFillsByPositionDemandId(demand.PositionDemandId))
                {
                    if (openDate == null)
                        openDate = fill.TimeStamp;
                    if (fill.PartInPosition == PartInPosition.PerpOpenSell)
                    {
                        perpSellPrice = ((totalSellPerp * perpSellPrice) + (fill.Price * (fill.Filled * fill.Lot))) / (totalSellPerp + (fill.Filled * fill.Lot));
                        totalSellPerp += (fill.Filled * fill.Lot);
                        totalFeeUsdt += fill.Fee;
                    }
                    if (fill.PartInPosition == PartInPosition.PerpCloseSell)
                    {
                        perpBuyPrice = ((totalBuyPerp * perpBuyPrice) + (fill.Price * (fill.Filled * fill.Lot))) / (totalBuyPerp + (fill.Filled * fill.Lot));
                        totalBuyPerp += (fill.Filled * fill.Lot);
                        totalFeeUsdt += fill.Fee;
                    }

                    if (fill.PartInPosition == PartInPosition.SpotBuy)
                    {
                        spotBuyPrice = ((totalBuySpot * spotBuyPrice) + (fill.Price * fill.Filled)) / (totalBuySpot + fill.Filled);
                        totalBuySpot += (fill.Filled);
                    }
                    if (fill.PartInPosition == PartInPosition.SpotSell)
                    {
                        spotSellPrice = ((totalSellSpot * spotSellPrice) + (fill.Price * fill.Filled)) / (totalSellSpot + fill.Filled);
                        totalSellSpot += (fill.Filled);
                        totalFeeUsdt += fill.Fee;
                    }
                }
            }
            if (totalSellPerp - totalBuyPerp != 0)
            {
                _logger.LogInformation("Still some of the position is not closed. total perp sold {totalSellPerp} total perp bought {totalBuyPerp}", totalSellPerp, totalBuyPerp);
                currentDemand.UpdateDate = DateTime.UtcNow;
                await _positionDemandRepository.UpdateAsync(currentDemand);
                return;
            }
            _logger.LogInformation("Need to close the whole position {Symbol}", currentDemand.Symbol);
            foreach (var demand in notDoneDemands)
            {
                demand.PositionDemandState = PositionDemandState.Done;
                demand.UpdateDate = DateTime.UtcNow;
                await _positionDemandRepository.UpdateAsync(demand);
            }
            totalFunding = await _fundingIncomeRepository.GetPositionFunding(currentDemand.Symbol);

            var spotTradePnl = (spotSellPrice * totalSellSpot) - (spotBuyPrice * totalBuySpot);
            var perpTradePnl = (perpSellPrice * totalSellPerp) - (perpBuyPrice * totalBuyPerp);
            var tradePnl = spotTradePnl + perpTradePnl;
            var totalPnl = tradePnl + totalFeeUsdt + totalFunding;

            var positionHistory = new PositionHistory()
            {
                OpenDate = openDate.Value,
                CloseDate = DateTime.UtcNow,
                Fee = totalFeeUsdt,
                Funding = totalFunding,
                PerpTrade = perpTradePnl,
                SpotTrade = spotTradePnl,
                Symbol = currentDemand.Symbol,
                TotalPNL = totalPnl
            };

            _logger.LogInformation("spotTradePnl {spotTradePnl}, perpTradePnl {perpTradePnl}, tradePnl {tradePnl}, totalFeeUsdt {totalFeeUsdt}, totalFunding {totalFunding}, totalPnl {totalPnl}", spotTradePnl, perpTradePnl, tradePnl, totalFeeUsdt, totalFunding, totalPnl);
            await _positionHistoryRepository.AddAsync(positionHistory);
            await _fundingIncomeRepository.EndCurrentPositionStatus(currentDemand.Symbol);
            return;
        }
    }
}

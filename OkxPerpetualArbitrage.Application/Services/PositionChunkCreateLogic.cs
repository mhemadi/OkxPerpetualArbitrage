using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OkxPerpetualArbitrage.Application.Contracts.Logic;
using OkxPerpetualArbitrage.Application.Contracts.OkxApi;
using OkxPerpetualArbitrage.Application.Contracts.Persistance;
using OkxPerpetualArbitrage.Application.Models.InfrastructureSettings;
using OkxPerpetualArbitrage.Application.Models.OkexApi;
using OkxPerpetualArbitrage.Application.Models.OkexApi.Enums;
using OkxPerpetualArbitrage.Domain.Entities;
using OkxPerpetualArbitrage.Domain.Entities.Enums;

namespace OkxPerpetualArbitrage.Application.Services
{
    public class PositionChunkCreateLogic : IPositionChunkCreateLogic
    {
        private readonly ILogger<PositionChunkCreateLogic> _logger;
        private readonly IPositionDemandRepository _positionDemandRepository;
        private readonly IOkxApiWrapper _apiWrapper;
        private readonly IOrderCreateLogic _orderCreateLogic;
        private readonly IOrderStateCheckLogic _orderStateCheckLogic;
        private readonly IOrderFillCreateLogic _orderFillCreateLogic;
        private readonly GeneralSetting _setting;


        public PositionChunkCreateLogic(ILogger<PositionChunkCreateLogic> logger, IPositionDemandRepository positionDemandRepository,
          IOkxApiWrapper apiWrapper, IOptions<GeneralSetting> setting, IOrderCreateLogic orderCreateLogic, IOrderStateCheckLogic orderStateCheckLogic,
          IOrderFillCreateLogic orderFillCreateLogic)
        {
            _logger = logger;
            _positionDemandRepository = positionDemandRepository;
            _apiWrapper = apiWrapper;
            _orderCreateLogic = orderCreateLogic;
            _orderStateCheckLogic = orderStateCheckLogic;
            _orderFillCreateLogic = orderFillCreateLogic;
            _setting = setting.Value;
        }

        public async Task<decimal> OpenClosePositionChunck(string symbol, decimal lotSizeChunk, int positionDemandId, decimal minSpread, PotentialPosition potentialPosition, bool tryToBeMaker, bool open)
        {

            int maxTries = _setting.MaxCloseTries;
            if (open)
                maxTries = _setting.MaxOpenTries;
            //var tmp = await _positionDemandRepository.GetIncompleteDemandsNoTracking(symbol);
            var demand = await _positionDemandRepository.GetPositionDemandNoTracking(positionDemandId);
            if (demand == null)
            {
                _logger.LogError("Could not find the demand to close the position chunck {symbol} {positionDemandId} ", symbol, positionDemandId);
                throw new Exception("Could not find the demand to close the position chunck");
            }
            string perpInstrumnet = _apiWrapper.GetPerpInstrument(symbol);
            string spotInstrumnet = _apiWrapper.GetSpotInstrument(symbol);
            decimal filledLotPerp = 0;
            string perpOrderId = "-1";
            string spotOrderId = "-1";
            int tries = 0;
            int spotTries;
            int spotLimitTries = 0;
            int maxSpotLimitTries = _setting.LimitOrderTriesBeforeMarketOrder;
            OKEXOrder perpOrder;
            OKEXOrder spotOrder;
            decimal spotSize;
            _apiWrapper.SetWait(500);
            _apiWrapper.SetMaxTry(100);
            while (filledLotPerp < lotSizeChunk)
            {

                tries++;
                demand = await _positionDemandRepository.GetPositionDemandNoTracking(positionDemandId);
                if (perpOrderId == "-1")
                {
                    if (tries >= maxTries || demand.IsCanceled)
                        break;
                    perpOrderId = await _orderCreateLogic.CreatePerpOrder(symbol, minSpread, lotSizeChunk - filledLotPerp, potentialPosition, tryToBeMaker, open);
                    if (perpOrderId == "-1")
                        continue;
                }
                await Task.Delay(100);

                perpOrder = await _apiWrapper.GetOrder(perpInstrumnet, perpOrderId);
                if (perpOrder == null)
                {
                    _logger.LogCritical("Failed to retrieve {perpOrderId} for {symbol}", perpOrderId, symbol);
                    bool tmpCancel = await _apiWrapper.CancelOrder(perpInstrumnet, perpOrderId);
                    if (!tmpCancel)
                    {
                        _logger.LogCritical("Failed to cancel {perpOrderId} for {symbol}. Exiting the method", perpOrderId, symbol);
                        return filledLotPerp;
                    }
                    perpOrder = await _apiWrapper.GetOrder(perpInstrumnet, perpOrderId);
                    if (perpOrder == null)
                    {
                        _logger.LogCritical("Failed to retrieve {perpOrderId} for {symbol} even after canceling it", perpOrderId, symbol);
                        return filledLotPerp;
                    }
                    if (perpOrder.Size == 0)
                    {
                        _logger.LogWarning("Canceled {perpOrderId} and continuing the loop", perpOrderId);
                        perpOrderId = "-1";
                        continue;
                    }
                }

                if (!tryToBeMaker || await _orderStateCheckLogic.CanKeepOrderPerp(symbol, perpOrder, minSpread, open) == false || perpOrder.Size != 0)
                {
                    bool wasCanceled = await _apiWrapper.CancelOrder(perpInstrumnet, perpOrderId);
                    if (!wasCanceled)
                    {
                        _logger.LogCritical("Failed to cancel {perpOrderId} for {symbol}. Continuing the loop", perpOrderId, symbol);
                        continue;
                    }
                    await Task.Delay(10);
                    perpOrder = await _apiWrapper.GetOrder(perpInstrumnet, perpOrderId);
                    if (perpOrder == null)
                    {
                        _logger.LogCritical("Failed to retrieve {perpOrderId} for {symbol}  after canceling it. Exiting the method ", perpOrderId, symbol);
                        return filledLotPerp;
                    }
                    perpOrderId = "-1";
                    if (perpOrder.Size == 0)
                        continue;
                    filledLotPerp += perpOrder.Size;
                    var part = PartInPosition.PerpCloseSell;
                    if (open)
                        part = PartInPosition.PerpOpenSell;
                    await _orderFillCreateLogic.AddOrderFill(perpOrder, demand.PositionDemandId, part, potentialPosition);
                    var idAndSize = await _orderCreateLogic.CreateFirstSpotOrder(symbol, perpOrder.Size, potentialPosition, tryToBeMaker, open);
                    spotOrderId = idAndSize.Item1;
                    spotSize = idAndSize.Item2;
                    if (spotOrderId == "-1")
                    {
                        _logger.LogCritical("Failed placing the spot order for {symbol} and {perpOrderId}. Buy {spotSize] worth of the asset and or close the perp position manualy from the exchange and update the records in the db ", symbol, perpOrderId, perpOrder.Size * potentialPosition.ContractValuePerp);
                        return filledLotPerp;
                    }
                    spotTries = 0;
                    spotLimitTries = 0;
                    while (spotSize > 0)
                    {



                        if (spotTries >= maxTries)
                        {
                            _logger.LogCritical("Failed filling the spot order for {symbol} and {perpOrderId}. Buy {spotSize] worth of the asset and or close the perp position manualy from the exchange and update the records in the db ", symbol, perpOrderId, spotSize);
                            return filledLotPerp;
                        }

                        await Task.Delay(25);
                        spotLimitTries++;
                        if (spotLimitTries >= maxSpotLimitTries)
                        {

                            if (await _apiWrapper.CancelOrder(spotInstrumnet, spotOrderId) == false)
                            {
                                _logger.LogCritical("Failed to cancel {spotOrderId} for {symbol}. Continuing the loop", spotOrderId, symbol);
                                spotTries++;
                                continue;
                            }
                            await Task.Delay(100);
                            spotOrder = await _apiWrapper.GetOrder(spotInstrumnet, spotOrderId);
                            if (spotOrder == null)
                            {
                                _logger.LogCritical("Failed to retrieve {spotOrderId} for {symbol}  after canceling it. Continuing the loop", spotOrderId, symbol);
                                spotTries++;
                                continue;
                            }
                            if (spotOrder.Size > 0)
                            {
                                spotSize -= spotOrder.Size;
                                //changes
                                part = PartInPosition.SpotSell;
                                if (open)
                                    part = PartInPosition.SpotBuy;
                                await _orderFillCreateLogic.AddOrderFill(spotOrder, demand.PositionDemandId, part, potentialPosition);
                                if (spotSize == 0)
                                    continue;
                            }
                            if (spotSize < potentialPosition.MinSizeSpot)
                            {
                                if (open)
                                    spotSize = potentialPosition.MinSizeSpot;
                                else
                                {
                                    spotSize = 0;
                                    continue;
                                }
                            }
                            if (!open)
                            {
                                spotOrderId = await _apiWrapper.PlaceOrder(spotInstrumnet, OKEXOrderType.market, OKEXTadeMode.cross, OKEXOrderSide.sell, OKEXPostitionSide.NotPosition, spotSize, 0, false);
                            }

                            if (open)
                            {
                                var ob = await _apiWrapper.GetOrderBook(spotInstrumnet);
                                var tmpPrice = spotOrder.InitialPrice * 1.04m;
                                if (ob != null)
                                    tmpPrice = ob.Asks[0].Price * 1.03m;
                                spotOrderId = await _apiWrapper.PlaceOrder(spotInstrumnet, OKEXOrderType.limit, OKEXTadeMode.cross, OKEXOrderSide.buy, OKEXPostitionSide.NotPosition, spotSize, tmpPrice, false);
                            }

                            if (spotOrderId == "-1")
                            {
                                _logger.LogCritical("Failed creating the final market spot order for {symbol} and {perpOrderId}. Buy {spotSize] worth of the asset and or close the perp position manualy from the exchange and update the records in the db ", symbol, perpOrderId, spotSize);
                                return filledLotPerp;
                            }

                            await Task.Delay(100);
                            spotOrder = await _apiWrapper.GetOrder(spotInstrumnet, spotOrderId);
                            if (spotOrder == null)
                            {
                                _logger.LogCritical("Failed getting the final market spot order for {symbol} and {spotOrderId]. Make sure all orders are filled and add the record to orderfill manualy", symbol, spotOrderId);
                                return filledLotPerp;
                            }
                            if (spotOrder.Size > 0)
                            {
                                spotSize -= spotOrder.Size;
                                part = PartInPosition.SpotSell;
                                if (open)
                                    part = PartInPosition.SpotBuy;
                                await _orderFillCreateLogic.AddOrderFill(spotOrder, demand.PositionDemandId, part, potentialPosition);

                            }
                            continue;
                        }


                        await Task.Delay(10);
                        spotOrder = await _apiWrapper.GetOrder(spotInstrumnet, spotOrderId);
                        if (spotOrder == null)
                        {
                            _logger.LogCritical("Failed to retrieve {spotOrderId} for {symbol}. Continuing the loop", spotOrderId, symbol);
                            spotTries++;
                            continue;
                        }
                        bool isValid = false;
                        if (!open)
                            isValid = await _orderStateCheckLogic.IsSpotBelowMid(symbol, spotOrder, potentialPosition);
                        else
                            isValid = await _orderStateCheckLogic.IsSpotAboveMid(symbol, spotOrder, potentialPosition);

                        if (!isValid)
                        {
                            if (await _apiWrapper.CancelOrder(spotInstrumnet, spotOrderId) == false)
                            {
                                _logger.LogCritical("Failed to cancel {spotOrderId} for {symbol}. Continuing the loop", spotOrderId, symbol);
                                spotTries++;
                                continue;
                            }
                            await Task.Delay(100);
                            spotOrder = await _apiWrapper.GetOrder(spotInstrumnet, spotOrderId);
                            if (spotOrder == null)
                            {
                                _logger.LogCritical("Failed to retrieve {spotOrderId} for {symbol}  after canceling it. Continuing the loop", spotOrderId, symbol);
                                spotTries++;
                                continue;
                            }
                            if (spotOrder.Size > 0)
                            {
                                spotSize -= spotOrder.Size;
                                part = PartInPosition.SpotSell;
                                if (open)
                                    part = PartInPosition.SpotBuy;
                                await _orderFillCreateLogic.AddOrderFill(spotOrder, demand.PositionDemandId, part, potentialPosition);
                                if (spotSize == 0)
                                    continue;
                            }
                            if (spotSize < potentialPosition.MinSizeSpot)
                            {
                                if (open)
                                    spotSize = potentialPosition.MinSizeSpot;
                                else
                                {
                                    spotSize = 0;
                                    continue;
                                }
                            }
                            spotOrderId = await _orderCreateLogic.CreateAdditionalSpotOrder(symbol, potentialPosition, spotSize, tryToBeMaker, open);
                            if (spotOrderId == "-1")
                            {
                                _logger.LogCritical("Failed creating the additional spot order for {symbol} and {perpOrderId}. Buy {spotSize] worth of the asset and or close the perp position manualy from the exchange and update the records in the db ", symbol, perpOrderId, spotSize);
                                return filledLotPerp;
                            }

                        }
                    }
                }
            }
            return filledLotPerp;
        }
    }
}

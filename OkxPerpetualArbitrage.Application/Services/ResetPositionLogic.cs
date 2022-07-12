using Microsoft.Extensions.Logging;
using OkxPerpetualArbitrage.Application.Contracts.Logic;
using OkxPerpetualArbitrage.Application.Contracts.OkxApi;
using OkxPerpetualArbitrage.Application.Contracts.Persistance;
using OkxPerpetualArbitrage.Application.Models.DTOs;

namespace OkxPerpetualArbitrage.Application.Services
{
    public class ResetPositionLogic : IResetPositionLogic
    {
        private readonly IPositionDemandRepository _positionDemandRepository;
        private readonly IOrderFillRepository _orderFillRepository;
        private readonly IFundingIncomeRepository _fundingIncomeRepository;
        private readonly IPotentialPositionRepository _potentialPositionRepository;
        private readonly IOkxApiWrapper _apiService;
        private readonly ILogger<ResetPositionLogic> _logger;

        public ResetPositionLogic(IPositionDemandRepository positionDemandRepository, IOrderFillRepository orderFillRepository, IFundingIncomeRepository fundingIncomeRepository, IPotentialPositionRepository potentialPositionRepository
            , IOkxApiWrapper apiService, ILogger<ResetPositionLogic> logger)
        {
            _positionDemandRepository = positionDemandRepository;
            _orderFillRepository = orderFillRepository;
            _fundingIncomeRepository = fundingIncomeRepository;
            _potentialPositionRepository = potentialPositionRepository;
            _apiService = apiService;
            _logger = logger;
        }

        public async Task<ApiCommandResponseDto> ResetPosition(string symbol, CancellationToken cancellationToken)
        {
            string msg = "";
            var unDones = await _positionDemandRepository.GetIncompleteDemandsNoTracking(symbol);
            foreach (var demand in unDones)
            {
                if (!demand.IsCanceled)
                {
                    await _positionDemandRepository.SetIsCanceled(demand.PositionDemandId, true);
                }
            }
            unDones = await _positionDemandRepository.GetIncompleteDemandsNoTracking(symbol);
            foreach (var demand in unDones)
            {
                var minSinceUpdate = DateTime.UtcNow.Subtract(demand.UpdateDate);
                if (minSinceUpdate.TotalMinutes < 5)
                    return new ApiCommandResponseDto() { Success = false, Message = "There has been an update recently. please wait for " + (5 - minSinceUpdate.TotalMinutes) + " minutes" };
                if (!demand.IsCanceled)
                    return new ApiCommandResponseDto() { Success = false, Message = "Not all requests are canceled" };
            }

            decimal positonSize = await _apiService.GetPositionSize(symbol);
            string orderId = "0";
            if (positonSize != 0)
            {
                orderId = await _apiService.PlaceOrder(_apiService.GetPerpInstrument(symbol), Models.OkexApi.Enums.OKEXOrderType.market, Models.OkexApi.Enums.OKEXTadeMode.cross, Models.OkexApi.Enums.OKEXOrderSide.buy, Models.OkexApi.Enums.OKEXPostitionSide.SHORT, positonSize, 0, true);
                msg += $"closed perp position of size {positonSize} . ";
            }
            if (orderId == "-1")
                return new ApiCommandResponseDto() { Success = false, Message = "Failed to close position" };

            var pp = await _potentialPositionRepository.GetPotentialPosition(symbol);
            var balance = await _apiService.GetBalance(symbol);
            decimal spotSize = balance == null ? 0 : balance.Cash;
            if (spotSize > pp.MinSizeSpot)
            {
                orderId = await _apiService.PlaceOrder(_apiService.GetSpotInstrument(symbol), Models.OkexApi.Enums.OKEXOrderType.market, Models.OkexApi.Enums.OKEXTadeMode.cross, Models.OkexApi.Enums.OKEXOrderSide.sell, Models.OkexApi.Enums.OKEXPostitionSide.NotPosition, spotSize, 0, false);
                msg += $"sold asset in the amount of {spotSize} . ";
            }
            if (spotSize < 0)
            {
                spotSize *= -1;
                if (spotSize < pp.MinSizeSpot)
                    spotSize = pp.MinSizeSpot;
                var ob = await _apiService.GetOrderBook(_apiService.GetSpotInstrument(symbol));
                var tmpPrice = ob.Asks[0].Price * 1.03m;
                orderId = await _apiService.PlaceOrder(_apiService.GetSpotInstrument(symbol), Models.OkexApi.Enums.OKEXOrderType.limit, Models.OkexApi.Enums.OKEXTadeMode.cross, Models.OkexApi.Enums.OKEXOrderSide.buy, Models.OkexApi.Enums.OKEXPostitionSide.NotPosition, spotSize, tmpPrice, false);
                msg += $"bought asset in the amount of {spotSize} . ";
            }
            if (orderId == "-1")
                return new ApiCommandResponseDto() { Success = false, Message = "Failed to settle spot" };

            foreach (var demand in unDones)
            {
                var fills = await _orderFillRepository.GetOrderFillsByPositionDemandId(demand.PositionDemandId);
                for (int i = 0; i < fills.Count; i++)
                {
                    await _orderFillRepository.DeleteAsync(fills[i]);
                    msg += $"removed order fill from db . ";
                }
            }

            for (int i = 0; i < unDones.Count; i++)
            {
                await _positionDemandRepository.DeleteAsync(unDones[i]);
                msg += $"removed position demand from db . ";
            }

            await _fundingIncomeRepository.DeleteInPosition(symbol);
            if (string.IsNullOrEmpty(msg))
                msg = "There was nothing to reset";

            return new ApiCommandResponseDto() { Success = true, Message = "Symbol has been reset. " + msg };
        }
    }
}

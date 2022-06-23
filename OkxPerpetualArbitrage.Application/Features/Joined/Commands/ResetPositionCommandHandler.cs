using MediatR;
using Microsoft.Extensions.Logging;
using OkxPerpetualArbitrage.Application.Contracts.ApiService;
using OkxPerpetualArbitrage.Application.Contracts.Persistance;
using OkxPerpetualArbitrage.Application.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OkxPerpetualArbitrage.Application.Features.Joined.Commands
{
    /// <summary>
    /// Handles the request to fully reset a postion
    /// All the orders related to the symbol will be canceled and closed
    /// All the records of the symbol will be erased from the database
    /// This should only be used manually if there is a critical error 
    /// </summary>
    public class ResetPositionCommandHandler : IRequestHandler<ResetPositionCommand, ApiCommandResponseDto>
    {
        private readonly IPositionDemandRepository _positionDemandRepository;
        private readonly IOrderFillRepository _orderFillRepository;
        private readonly IFundingIncomeRepository _fundingIncomeRepository;
        private readonly IPotentialPositionRepository _potentialPositionRepository;
        private readonly IApiService _apiService;
        private readonly ILogger<ResetPositionCommandHandler> _logger;

        public ResetPositionCommandHandler(IPositionDemandRepository positionDemandRepository, IOrderFillRepository orderFillRepository, IFundingIncomeRepository fundingIncomeRepository, IPotentialPositionRepository potentialPositionRepository
            , IApiService apiService, ILogger<ResetPositionCommandHandler> logger)
        {
            _positionDemandRepository = positionDemandRepository;
            _orderFillRepository = orderFillRepository;
            _fundingIncomeRepository = fundingIncomeRepository;
            _potentialPositionRepository = potentialPositionRepository;
            _apiService = apiService;
            _logger = logger;
        }
        public async Task<ApiCommandResponseDto> Handle(ResetPositionCommand request, CancellationToken cancellationToken)
        {
            string msg = "";
            var unDones = await _positionDemandRepository.GetIncompleteDemandsNoTracking(request.Symbol);
            foreach (var demand in unDones)
            {
                if (!demand.IsCanceled)
                {
                    await _positionDemandRepository.SetIsCanceled(demand.PositionDemandId, true);

                }
            }
            unDones = await _positionDemandRepository.GetIncompleteDemandsNoTracking(request.Symbol);
            foreach (var demand in unDones)
            {
                var minSinceUpdate = DateTime.UtcNow.Subtract(demand.UpdateDate);
                if (minSinceUpdate.TotalMinutes < 5)
                    return new ApiCommandResponseDto() { Success = false, Message = "There has been an update recently. please wait for " + (5 - minSinceUpdate.TotalMinutes) + " minutes" };
                if (!demand.IsCanceled)
                    return new ApiCommandResponseDto() { Success = false, Message = "Not all requests are canceled" };
            }


            //make sure size is negative for sell orders on both spot and perp

            decimal positonSize = await _apiService.GetPositionSize(request.Symbol);
            string orderId = "0";
            if (positonSize != 0)
            {
                orderId = await _apiService.PlaceOrder(_apiService.GetPerpInstrument(request.Symbol), Models.OkexApi.Enums.OKEXOrderType.market, Models.OkexApi.Enums.OKEXTadeMode.cross, Models.OkexApi.Enums.OKEXOrderSide.buy, Models.OkexApi.Enums.OKEXPostitionSide.SHORT, positonSize, 0, true);
                msg += $"closed perp position of size {positonSize} . ";
            }
            if (orderId == "-1")
                return new ApiCommandResponseDto() { Success = false, Message = "Failed to close position" };



            var pp = await _potentialPositionRepository.GetPotentialPosition(request.Symbol);

            var balance = await _apiService.GetBalance(request.Symbol);
            decimal spotSize = balance == null ? 0 : balance.Cash;
            if (spotSize > pp.MinSizeSpot)
            {
                orderId = await _apiService.PlaceOrder(_apiService.GetSpotInstrument(request.Symbol), Models.OkexApi.Enums.OKEXOrderType.market, Models.OkexApi.Enums.OKEXTadeMode.cross, Models.OkexApi.Enums.OKEXOrderSide.sell, Models.OkexApi.Enums.OKEXPostitionSide.NotPosition, spotSize, 0, false);
                msg += $"sold asset in the amount of {spotSize} . ";
            }
            if (spotSize < 0)
            {
                spotSize *= -1;
                if (spotSize < pp.MinSizeSpot)
                    spotSize = pp.MinSizeSpot;
                var ob = await _apiService.GetOrderBook(_apiService.GetSpotInstrument(request.Symbol));
                var tmpPrice = ob.Asks[0].Price * 1.03m;
                orderId = await _apiService.PlaceOrder(_apiService.GetSpotInstrument(request.Symbol), Models.OkexApi.Enums.OKEXOrderType.limit, Models.OkexApi.Enums.OKEXTadeMode.cross, Models.OkexApi.Enums.OKEXOrderSide.buy, Models.OkexApi.Enums.OKEXPostitionSide.NotPosition, spotSize, tmpPrice, false);
                msg += $"sold asset in the amount of {spotSize} . ";
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


            await _fundingIncomeRepository.DeleteInPosition(request.Symbol);
            if (string.IsNullOrEmpty(msg))
                msg = "There was nothing to reset";

            return new ApiCommandResponseDto() { Success = true, Message = "Symbol has been reset. " + msg };
        }
    }
}

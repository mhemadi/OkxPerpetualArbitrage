using Microsoft.Extensions.Logging;
using OkxPerpetualArbitrage.Application.Contracts.ApiService;
using OkxPerpetualArbitrage.Application.Contracts.Logic;
using OkxPerpetualArbitrage.Application.Contracts.Persistance;
using OkxPerpetualArbitrage.Application.Helpers;
using OkxPerpetualArbitrage.Application.Models.DTOs;
using OkxPerpetualArbitrage.Application.Models.OkexApi.Enums;
using OkxPerpetualArbitrage.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OkxPerpetualArbitrage.Application.Services
{
    public class PositionOpenCloseValidationLogic : IPositionOpenCloseValidationLogic
    {


        private readonly ILogger<PositionOpenCloseValidationLogic> _logger;
        private readonly ITotalAvailableCloseSizeCalculator _totalAvailableCloseSizeCalculator;
        private readonly IPositionDemandRepository _positionDemandRepository;
        private readonly IApiService _apiService;
        private readonly IInProgressDemandLogic _inProgressDemandLogic;

        public PositionOpenCloseValidationLogic(ILogger<PositionOpenCloseValidationLogic> logger, ITotalAvailableCloseSizeCalculator totalAvailableCloseSizeCalculator
            , IPositionDemandRepository positionDemandRepository, IApiService apiService, IInProgressDemandLogic inProgressDemandLogic)
        {
            _logger = logger;
            _totalAvailableCloseSizeCalculator = totalAvailableCloseSizeCalculator;
            _positionDemandRepository = positionDemandRepository;
            _apiService = apiService;
            _inProgressDemandLogic = inProgressDemandLogic;
        }

        public async Task<ApiCommandResponseDto> PreValidateOpen(OpenPositionDto openPositionDto, PotentialPosition pp)
        {

            var r = await PreValidate(openPositionDto.Symbol, openPositionDto.Size, pp);
            if (!r.Success)
                return r;

            var balance = await _apiService.GetUSDTBalance();
            if (balance.Available * 0.95m < await _inProgressDemandLogic.GetTotalInProgressRequiredFunds() + (openPositionDto.Size * pp.MarkPrice))
                return new ApiCommandResponseDto() { Success = false, Message = "Not enough funds" };


            if (await TestOrder(openPositionDto.Symbol, openPositionDto.Size, pp, true) == false)
                return new ApiCommandResponseDto() { Success = false, Message = "Could not place orders. Please make sure the api keys have trading permissions" };

            return new ApiCommandResponseDto() { Success = true, Message = "" };

        }

        public async Task<ApiCommandResponseDto> PreValidateClose(ClosePositionDto closePositionDto, PotentialPosition pp)
        {

            var r = await PreValidate(closePositionDto.Symbol, closePositionDto.Size, pp);
            if (!r.Success)
                return r;

            if (closePositionDto.Size > await _totalAvailableCloseSizeCalculator.GetTotalAvailableSize(closePositionDto.Symbol))
                return new ApiCommandResponseDto() { Success = false, Message = "Size larger than available" };

            if (await TestOrder(closePositionDto.Symbol, closePositionDto.Size, pp, false) == false)
                return new ApiCommandResponseDto() { Success = false, Message = "Could not place orders. Please make sure the api keys have trading permissions" };

            return new ApiCommandResponseDto() { Success = true, Message = "" };
        }

        public async Task<ApiCommandResponseDto> PostValidateOpen(OpenPositionDto openPositionDto, int positionDemandId, PotentialPosition pp)
        {
            if (await _positionDemandRepository.IsPositionDemandInProgress(openPositionDto.Symbol, positionDemandId))
                return new ApiCommandResponseDto() { Success = false, Message = "There is already a progress request for this symbool. Please wait" };

            var balance = await _apiService.GetUSDTBalance();
            if (balance.Available * 0.95m < await _inProgressDemandLogic.GetTotalInProgressRequiredFunds())
                return new ApiCommandResponseDto() { Success = false, Message = "Not enough funds" };

            return new ApiCommandResponseDto() { Success = true, Message = "" };
        }

        public async Task<ApiCommandResponseDto> PostValidateClose(ClosePositionDto closePositionDto, int positionDemandId, PotentialPosition pp)
        {
            if (await _positionDemandRepository.IsPositionDemandInProgress(closePositionDto.Symbol, positionDemandId))
                return new ApiCommandResponseDto() { Success = false, Message = "There is an in progress request for this symbool. Please wait" };


            if (closePositionDto.Size > await _totalAvailableCloseSizeCalculator.GetTotalAvailableSize(closePositionDto.Symbol))
                return new ApiCommandResponseDto() { Success = false, Message = "Size larger than available" };


            return new ApiCommandResponseDto() { Success = true, Message = "" };
        }

        private async Task<ApiCommandResponseDto> PreValidate(string symbol, decimal size, PotentialPosition pp)
        {
            if (DateTimeHelper.TimeToFunding().TotalMinutes < 10)
                return new ApiCommandResponseDto() { Success = false, Message = "Too close to funding time. Try again later" };

            if (await _positionDemandRepository.IsPositionDemandInProgress(symbol))
                return new ApiCommandResponseDto() { Success = false, Message = "There is already a progress request for this symbool. Please wait" };
            if (pp == null)
                return new ApiCommandResponseDto() { Success = false, Message = "Could not find the symbol" };

            if (size < pp.ContractValuePerp || size % pp.ContractValuePerp != 0)
                return new ApiCommandResponseDto() { Success = false, Message = "size must be a multiple of " + pp.ContractValuePerp };

            var perpContract = await _apiService.GetPerpContractData(symbol);
            if (perpContract.Item3 != pp.ContractValuePerp)
                return new ApiCommandResponseDto() { Success = false, Message = "Contract value has changed. Please try again later" };

            return new ApiCommandResponseDto() { Success = true, Message ="" };

        }

        private async Task<bool> TestOrder(string symbol, decimal size, PotentialPosition pp, bool open)
        {

            string perpInstrumnet = _apiService.GetPerpInstrument(symbol);
            string spotInstrumnet = _apiService.GetSpotInstrument(symbol);
            var spotOrderBook = await _apiService.GetOrderBook(spotInstrumnet);
            var perpOrderBook = await _apiService.GetOrderBook(perpInstrumnet);

            decimal perpPrice = perpOrderBook.Asks[0].Price * 1.05m;

            OKEXOrderSide side = OKEXOrderSide.sell;
            if (!open)
            {
                side = OKEXOrderSide.buy;
                perpPrice = perpOrderBook.Asks[0].Price * 0.95m;
            }
            string perpOrderId = await _apiService.PlaceOrder(perpInstrumnet, OKEXOrderType.limit, OKEXTadeMode.cross, side, OKEXPostitionSide.SHORT, size / pp.ContractValuePerp, perpPrice, !open, 1, 10);
            if (perpOrderId == "-1")
                return false;
            if (!await _apiService.CancelOrder(perpInstrumnet, perpOrderId))
            {
                _logger.LogCritical("Failed to cancel test order perp");
                return false;
            }


            decimal spotPrice = spotOrderBook.Bids[0].Price * 0.95m;
            side = OKEXOrderSide.buy;
            if (!open)
            {
                side = OKEXOrderSide.sell;
                spotPrice = spotOrderBook.Bids[0].Price * 1.05m;
            }
            var spotOrderId = await _apiService.PlaceOrder(spotInstrumnet, OKEXOrderType.limit, OKEXTadeMode.cross, side, OKEXPostitionSide.NotPosition, size, spotPrice, false, 1, 10);
            if (spotOrderId == "-1")
                return false;

            if (!await _apiService.CancelOrder(spotInstrumnet, spotOrderId))
            {
                _logger.LogCritical("Failed to cancel test order spot");
                return false;
            }
            return true;


        }
    }
}

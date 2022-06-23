using Microsoft.Extensions.Logging;
using OkxPerpetualArbitrage.Application.Contracts.ApiService;
using OkxPerpetualArbitrage.Application.Contracts.Logic;
using OkxPerpetualArbitrage.Application.Contracts.Persistance;
using OkxPerpetualArbitrage.Application.Exceptions;
using OkxPerpetualArbitrage.Application.Features.Joined.Commands;
using OkxPerpetualArbitrage.Application.Models.DTOs;
using OkxPerpetualArbitrage.Domain.Entities;
using OkxPerpetualArbitrage.Domain.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OkxPerpetualArbitrage.Application.Services
{


    public class PositionOpenCloseChannelWriterLogic : IPositionOpenCloseChannelWriterLogic
    {
        private readonly IClosePositionProcessingChannel _closePositionProcessingChannel;
        private readonly ILogger<PositionOpenCloseChannelWriterLogic> _logger;
        private readonly IPositionChunckSizeCalculator _orderChunckSizeCalculator;
        private readonly IPotentialPositionRepository _potentialPositionRepository;
        private readonly IPositionDemandRepository _positionDemandRepository;
        private readonly IPositionOpenCloseValidationLogic _positionOpenCloseValidationLogic;
        private readonly IOpenPositionProcessingChannel _openPositionProcessingChannel;

        public PositionOpenCloseChannelWriterLogic(IClosePositionProcessingChannel closePositionProcessingChannel, ILogger<PositionOpenCloseChannelWriterLogic> logger, IPositionChunckSizeCalculator orderChunckSizeCalculator
            , IPotentialPositionRepository potentialPositionRepository, IPositionDemandRepository positionDemandRepository, IApiService apiService, IOrderFillRepository orderFillRepository, ITotalAvailableCloseSizeCalculator totalAvailableCloseSizeCalculator, IPositionOpenCloseValidationLogic positionOpenCloseValidationLogic
            , IOpenPositionProcessingChannel openPositionProcessingChannel)
        {
            _closePositionProcessingChannel = closePositionProcessingChannel;
            _logger = logger;
            _orderChunckSizeCalculator = orderChunckSizeCalculator;
            _potentialPositionRepository = potentialPositionRepository;
            _positionDemandRepository = positionDemandRepository;
            _positionOpenCloseValidationLogic = positionOpenCloseValidationLogic;
            _openPositionProcessingChannel = openPositionProcessingChannel;
        }
        public async Task<ApiCommandResponseDto> WriteToChannel(bool open, OpenClosePositionDto openClosePositionDto, CancellationToken cancellationToken)
        {
            var pp = await _potentialPositionRepository.GetPotentialPosition(openClosePositionDto.Symbol);
            if (pp == null)
            {
                _logger.LogError($"Could not get the potential positon for symbol {openClosePositionDto.Symbol} to handle close command");
                throw new OkxPerpetualArbitrageCustomException("Symbol not found");
            }
            ApiCommandResponseDto apiCommandResponseDto;
            if (open)
                apiCommandResponseDto = await _positionOpenCloseValidationLogic.PreValidateOpen(openClosePositionDto as OpenPositionDto, pp);
            else
                apiCommandResponseDto = await _positionOpenCloseValidationLogic.PreValidateClose(openClosePositionDto as ClosePositionDto, pp);
            if (!apiCommandResponseDto.Success)
                return apiCommandResponseDto;

            decimal lotSize = openClosePositionDto.Size / pp.ContractValuePerp;
            decimal chunkLotSize = _orderChunckSizeCalculator.GetChunkLotSize(pp, lotSize);

            PositionDemand demand = new PositionDemand()
            {
                Filled = 0,
                IsCanceled = false,
                IsInstant = false,
                Spread = openClosePositionDto.MinSpread,
                OpenDate = DateTime.UtcNow,
                PositionDemandSide = PositionDemandSide.Open,
                PositionDemandState = PositionDemandState.InProgress,
                Symbol = openClosePositionDto.Symbol,
                TotalSize = openClosePositionDto.Size,
                UpdateDate = DateTime.UtcNow
            };
            if (!open)
            {
                demand.IsInstant = (openClosePositionDto as ClosePositionDto).IsInstant;
                demand.PositionDemandSide = PositionDemandSide.Close;
            }
            await _positionDemandRepository.AddAsync(demand);

            if (open)
                apiCommandResponseDto = await _positionOpenCloseValidationLogic.PostValidateOpen(openClosePositionDto as OpenPositionDto, demand.PositionDemandId, pp);
            else
                apiCommandResponseDto = await _positionOpenCloseValidationLogic.PostValidateClose(openClosePositionDto as ClosePositionDto, demand.PositionDemandId, pp);
            if (!apiCommandResponseDto.Success)
            {
                await _positionDemandRepository.DeleteAsync(demand);
                return apiCommandResponseDto;
            }

            bool addedToChanne;
            if (!open)
            {
                addedToChanne = _closePositionProcessingChannel.AddClosePositionDemand(new Models.Channels.ClosePositionProcessingChannelDto
                {
                    LotSize = lotSize,
                    LotSizeChunck = chunkLotSize,
                    MinSpread = openClosePositionDto.MinSpread,
                    PositionDemandId = demand.PositionDemandId,
                    PotentialPosition = pp,
                    Symbol = openClosePositionDto.Symbol,
                    IsInstant = (openClosePositionDto as ClosePositionDto).IsInstant
                });
            }
            else
            {
                addedToChanne = _openPositionProcessingChannel.AddOpenPositionDemand(new Models.Channels.OpenPositionProcessingChannelDto
                {
                    LotSize = lotSize,
                    LotSizeChunck = chunkLotSize,
                    MinSpread = openClosePositionDto.MinSpread,
                    PositionDemandId = demand.PositionDemandId,
                    PotentialPosition = pp,
                    Symbol = openClosePositionDto.Symbol
                });
            }

            if (!addedToChanne)
            {
                await _positionDemandRepository.DeleteAsync(demand);
                return new ApiCommandResponseDto() { Success = false, Message = "Could not add your request to the queue" };
            }

            return new ApiCommandResponseDto() { Success = true, Message = "Close request has been submitted successfuly" };
        }





    }
}

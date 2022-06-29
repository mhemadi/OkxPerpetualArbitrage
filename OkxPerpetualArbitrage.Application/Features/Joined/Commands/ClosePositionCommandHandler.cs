using MediatR;
using Microsoft.Extensions.Logging;
using OkxPerpetualArbitrage.Application.Contracts.OkxApi;
using OkxPerpetualArbitrage.Application.Contracts.Logic;
using OkxPerpetualArbitrage.Application.Contracts.Persistance;
using OkxPerpetualArbitrage.Application.Exceptions;
using OkxPerpetualArbitrage.Application.Models.DTOs;
using OkxPerpetualArbitrage.Domain.Entities;
using OkxPerpetualArbitrage.Domain.Entities.Enums;

namespace OkxPerpetualArbitrage.Application.Features.Joined.Commands
{
    /// <summary>
    /// Handles the request to close a postion
    /// The request is forwarded to a channel to be proccessed in the background
    /// </summary>
    public class ClosePositionCommandHandler : IRequestHandler<ClosePositionCommand, ApiCommandResponseDto>
    {
        private readonly IPositionOpenCloseChannelWriterLogic _positionOpenCloseChannelWriterLogic;

        public ClosePositionCommandHandler(IPositionOpenCloseChannelWriterLogic positionOpenCloseChannelWriterLogic)
        {
            _positionOpenCloseChannelWriterLogic = positionOpenCloseChannelWriterLogic;
        }

        public Task<ApiCommandResponseDto> Handle(ClosePositionCommand request, CancellationToken cancellationToken)
        {
            return _positionOpenCloseChannelWriterLogic.WriteToChannel(false, request.ClosePositionDto, cancellationToken);
        }
    }
}

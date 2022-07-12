using MediatR;
using OkxPerpetualArbitrage.Application.Contracts.Logic;
using OkxPerpetualArbitrage.Application.Models.DTOs;

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

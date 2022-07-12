using MediatR;
using OkxPerpetualArbitrage.Application.Contracts.Logic;
using OkxPerpetualArbitrage.Application.Models.DTOs;

namespace OkxPerpetualArbitrage.Application.Features.Joined.Commands
{
    /// <summary>
    /// Handles the request to open a postion
    /// The request is forwarded to a channel to be proccessed in the background
    /// </summary>
    public class OpenPositionCommandHandler : IRequestHandler<OpenPositionCommand, ApiCommandResponseDto>
    {
        private readonly IPositionOpenCloseChannelWriterLogic _positionOpenCloseChannelWriterLogic;

        public OpenPositionCommandHandler(IPositionOpenCloseChannelWriterLogic positionOpenCloseChannelWriterLogic)
        {
            _positionOpenCloseChannelWriterLogic = positionOpenCloseChannelWriterLogic;
        }

        public Task<ApiCommandResponseDto> Handle(OpenPositionCommand request, CancellationToken cancellationToken)
        {
            return _positionOpenCloseChannelWriterLogic.WriteToChannel(true, request.OpenPositionDto, cancellationToken);

        }
    }
}

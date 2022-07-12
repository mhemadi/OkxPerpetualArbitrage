using MediatR;
using OkxPerpetualArbitrage.Application.Contracts.Logic;
using OkxPerpetualArbitrage.Application.Models.DTOs;

namespace OkxPerpetualArbitrage.Application.Features.PositionDemands.Commands
{
    /// <summary>
    /// Handles the request to cancel an open or closer request
    /// </summary>
    public class CancelDemandCommandHandler : IRequestHandler<CancelDemandCommand, ApiCommandResponseDto>
    {
        private readonly ICancelDemandLogic _cancelDemandLogic;

        public CancelDemandCommandHandler(ICancelDemandLogic cancelDemandLogic)
        {
            _cancelDemandLogic = cancelDemandLogic;
        }
        public async Task<ApiCommandResponseDto> Handle(CancelDemandCommand request, CancellationToken cancellationToken)
        {
            return await _cancelDemandLogic.Cancel(request.DemandId, cancellationToken);
        }
    }
}

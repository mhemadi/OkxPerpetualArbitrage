using MediatR;
using OkxPerpetualArbitrage.Application.Contracts.Logic;
using OkxPerpetualArbitrage.Application.Contracts.Persistance;
using OkxPerpetualArbitrage.Application.Models.DTOs;
using OkxPerpetualArbitrage.Domain.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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

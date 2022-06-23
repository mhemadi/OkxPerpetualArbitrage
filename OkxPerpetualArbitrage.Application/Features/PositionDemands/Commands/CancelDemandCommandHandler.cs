using MediatR;
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
        private readonly IPositionDemandRepository _positionDemandRepository;

        public CancelDemandCommandHandler(IPositionDemandRepository positionDemandRepository)
        {
            _positionDemandRepository = positionDemandRepository;
        }
        public async Task<ApiCommandResponseDto> Handle(CancelDemandCommand request, CancellationToken cancellationToken)
        {
            var demand = await  _positionDemandRepository.GetByIdAsync(request.DemandId);
            if (demand == null)
                return new ApiCommandResponseDto() { Success = false, Message = "Can not find the request" };
            if (demand.PositionDemandState != PositionDemandState.InProgress)
                return new ApiCommandResponseDto() { Success = false, Message = "Request is already done and can not be canceled" };
            if (demand.IsCanceled)
                return new ApiCommandResponseDto() { Success = false, Message = "Request is already canceled and can not be canceled again" };
              await  _positionDemandRepository.SetIsCanceled(demand.PositionDemandId, true);
            return new ApiCommandResponseDto() { Success = true, Message = "Cancel request has been submitted successfuly" };
        }
    }
}

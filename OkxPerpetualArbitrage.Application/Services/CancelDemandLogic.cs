using OkxPerpetualArbitrage.Application.Contracts.Logic;
using OkxPerpetualArbitrage.Application.Contracts.Persistance;
using OkxPerpetualArbitrage.Application.Models.DTOs;
using OkxPerpetualArbitrage.Domain.Entities.Enums;

namespace OkxPerpetualArbitrage.Application.Services
{
    public class CancelDemandLogic : ICancelDemandLogic
    {
        private readonly IPositionDemandRepository _positionDemandRepository;

        public CancelDemandLogic(IPositionDemandRepository positionDemandRepository)
        {
            _positionDemandRepository = positionDemandRepository;
        }
        public async Task<ApiCommandResponseDto> Cancel(int demandId, CancellationToken cancellationToken)
        {
            var demand = await _positionDemandRepository.GetByIdAsync(demandId);
            if (demand == null)
                return new ApiCommandResponseDto() { Success = false, Message = "Can not find the request" };
            if (demand.PositionDemandState != PositionDemandState.InProgress)
                return new ApiCommandResponseDto() { Success = false, Message = "Request is already done and can not be canceled" };
            if (demand.IsCanceled)
                return new ApiCommandResponseDto() { Success = false, Message = "Request is already canceled and can not be canceled again" };
            await _positionDemandRepository.SetIsCanceled(demand.PositionDemandId, true);
            return new ApiCommandResponseDto() { Success = true, Message = "Cancel request has been submitted successfuly" };
        }
    }
}

using OkxPerpetualArbitrage.Application.Models.DTOs;

namespace OkxPerpetualArbitrage.Application.Contracts.Logic
{
    public interface ICancelDemandLogic
    {
        Task<ApiCommandResponseDto> Cancel(int demandId, CancellationToken cancellationToken);
    }
}

using OkxPerpetualArbitrage.Application.Models.DTOs;

namespace OkxPerpetualArbitrage.Application.Contracts.Logic
{
    public interface IResetPositionLogic
    {
        Task<ApiCommandResponseDto> ResetPosition(string symbol, CancellationToken cancellationToken);
    }
}

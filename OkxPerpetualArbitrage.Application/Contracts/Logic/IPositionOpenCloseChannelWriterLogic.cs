using OkxPerpetualArbitrage.Application.Models.DTOs;

namespace OkxPerpetualArbitrage.Application.Contracts.Logic
{
    public interface IPositionOpenCloseChannelWriterLogic
    {
        Task<ApiCommandResponseDto> WriteToChannel(bool open, OpenClosePositionDto openClosePositionDto, CancellationToken cancellationToken);
    }
}

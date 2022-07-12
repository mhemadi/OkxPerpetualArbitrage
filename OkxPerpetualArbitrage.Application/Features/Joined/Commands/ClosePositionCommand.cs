using MediatR;
using OkxPerpetualArbitrage.Application.Models.DTOs;

namespace OkxPerpetualArbitrage.Application.Features.Joined.Commands
{
    public class ClosePositionCommand : IRequest<ApiCommandResponseDto>
    {
        public ClosePositionDto ClosePositionDto { get; set; }
    }
}

using MediatR;
using OkxPerpetualArbitrage.Application.Models.DTOs;

namespace OkxPerpetualArbitrage.Application.Features.Joined.Commands
{
    public class OpenPositionCommand : IRequest<ApiCommandResponseDto>
    {
        public OpenPositionDto OpenPositionDto { get; set; }
    }
}

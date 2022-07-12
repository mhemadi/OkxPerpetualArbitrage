using MediatR;
using OkxPerpetualArbitrage.Application.Models.DTOs;

namespace OkxPerpetualArbitrage.Application.Features.Joined.Commands
{
    public class ResetPositionCommand : IRequest<ApiCommandResponseDto>
    {
        public string Symbol { get; set; }
    }
}

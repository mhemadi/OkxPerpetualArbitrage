using MediatR;
using OkxPerpetualArbitrage.Application.Models.DTOs;

namespace OkxPerpetualArbitrage.Application.Features.PositionDemands.Commands
{
    public class CancelDemandCommand : IRequest<ApiCommandResponseDto>
    {
        public int DemandId { get; set; }
    }
}

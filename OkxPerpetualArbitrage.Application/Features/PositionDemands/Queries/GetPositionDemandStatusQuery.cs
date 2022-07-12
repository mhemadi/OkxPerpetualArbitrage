using MediatR;
using OkxPerpetualArbitrage.Application.Models.DTOs;

namespace OkxPerpetualArbitrage.Application.Features.PositionDemands.Queries
{
    public class GetPositionDemandStatusQuery : IRequest<PositionDemandStatusDto>
    {
        public string Symbol { get; set; }
    }
}

using MediatR;
using OkxPerpetualArbitrage.Application.Models.DTOs;

namespace OkxPerpetualArbitrage.Application.Features.Joined.Queries
{
    public class GetPotentialPositionsQuery : IRequest<List<PotentialPositionListItemDto>>
    {

    }
}

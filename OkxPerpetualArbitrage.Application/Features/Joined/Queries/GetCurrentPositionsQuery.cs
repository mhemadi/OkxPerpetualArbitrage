using MediatR;
using OkxPerpetualArbitrage.Application.Models.DTOs;

namespace OkxPerpetualArbitrage.Application.Features.Joined.Queries
{
    public class GetCurrentPositionsQuery : IRequest<List<CurrentPositionListItemDto>>
    {
        public bool CheckError { get; set; } = false;
    }
}

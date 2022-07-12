using MediatR;
using OkxPerpetualArbitrage.Application.Contracts.Logic;
using OkxPerpetualArbitrage.Application.Models.DTOs;

namespace OkxPerpetualArbitrage.Application.Features.Joined.Queries
{
    /// <summary>
    /// Handles the request to get a list of all available symbols that a position may be opened on them
    /// </summary>
    public class GetPotentialPositionsQueryHandler : IRequestHandler<GetPotentialPositionsQuery, List<PotentialPositionListItemDto>>
    {
        private readonly IGetPotentialPositionsLogic _getPotentialPositionsLogic;

        public GetPotentialPositionsQueryHandler(IGetPotentialPositionsLogic getPotentialPositionsLogic)
        {
            _getPotentialPositionsLogic = getPotentialPositionsLogic;
        }
        public async Task<List<PotentialPositionListItemDto>> Handle(GetPotentialPositionsQuery request, CancellationToken cancellationToken)
        {
            return await _getPotentialPositionsLogic.GetPotentialPositions(cancellationToken);
        }
    }
}

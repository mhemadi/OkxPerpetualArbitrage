using MediatR;
using OkxPerpetualArbitrage.Application.Contracts.Logic;
using OkxPerpetualArbitrage.Application.Models.DTOs;


namespace OkxPerpetualArbitrage.Application.Features.Joined.Queries
{
    /// <summary>
    /// Handles the users request to get all open poitions
    /// </summary>
    public class GetCurrentPositionsQueryHandler : IRequestHandler<GetCurrentPositionsQuery, List<CurrentPositionListItemDto>>
    {
        private readonly IGetCurrentPositionsLogic _getCurrentPositionsLogic;

        public GetCurrentPositionsQueryHandler(IGetCurrentPositionsLogic getCurrentPositionsLogic)
        {
            _getCurrentPositionsLogic = getCurrentPositionsLogic;
        }
        public async Task<List<CurrentPositionListItemDto>> Handle(GetCurrentPositionsQuery request, CancellationToken cancellationToken)
        {
            return await _getCurrentPositionsLogic.GetCurrentPositions(request.CheckError, cancellationToken);
        }
    }
}

using MediatR;
using OkxPerpetualArbitrage.Application.Contracts.Logic;

namespace OkxPerpetualArbitrage.Application.Features.PotentialPositions.Commands
{
    /// <summary>
    /// Handles the request to get a list of all available symbols
    /// </summary>
    public class GetSymolsQueryHandler : IRequestHandler<GetSymolsQuery, List<string>>
    {
        private readonly IPotentialPositionProcessorLogic _potentialPositionProcessorLogic;

        public GetSymolsQueryHandler(IPotentialPositionProcessorLogic potentialPositionProcessorLogic)
        {
            _potentialPositionProcessorLogic = potentialPositionProcessorLogic;
        }
        public async Task<List<string>> Handle(GetSymolsQuery request, CancellationToken cancellationToken)
        {
            var all = await _potentialPositionProcessorLogic.GetAllPotentialPositions();
            return all.OrderBy(x => x.Symbol).Select(x => x.Symbol).ToList();
        }
    }
}

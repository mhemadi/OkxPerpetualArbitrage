using MediatR;
using OkxPerpetualArbitrage.Application.Contracts.Persistance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OkxPerpetualArbitrage.Application.Features.PotentialPositions.Commands
{
    /// <summary>
    /// Handles the request to get a list of all available symbols
    /// </summary>
    public class GetSymolsQueryHandler : IRequestHandler<GetSymolsQuery, List<string>>
    {
        private readonly IPotentialPositionRepository _potentialPositionRepository;

        public GetSymolsQueryHandler(IPotentialPositionRepository potentialPositionRepository)
        {
            _potentialPositionRepository = potentialPositionRepository;
        }
        public async Task<List<string>> Handle(GetSymolsQuery request, CancellationToken cancellationToken)
        {
           var all =  await _potentialPositionRepository.GetAllAsync();
            return all.OrderBy(x=>x.Symbol).Select(x => x.Symbol).ToList();
        }
    }
}

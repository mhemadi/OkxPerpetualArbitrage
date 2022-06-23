using AutoMapper;
using MediatR;
using OkxPerpetualArbitrage.Application.Contracts.Persistance;
using OkxPerpetualArbitrage.Application.Models.DTOs;
using OkxPerpetualArbitrage.Domain.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OkxPerpetualArbitrage.Application.Features.Joined.Queries
{
    /// <summary>
    /// Handles the request to get a list of all available symbols that a position may be opened on them
    /// </summary>
    public class GetPotentialPositionsQueryHandler : IRequestHandler<GetPotentialPositionsQuery, List<PotentialPositionListItemDto>>
    {
        private readonly IMapper _mapper;
        private readonly IPotentialPositionRepository _potentialPositionRepository;
        private readonly IPositionDemandRepository _positionDemandRepository;

        public GetPotentialPositionsQueryHandler(IMapper mapper, IPotentialPositionRepository potentialPositionRepository, IPositionDemandRepository positionDemandRepository)
        {
            _mapper = mapper;
            _potentialPositionRepository = potentialPositionRepository;
            _positionDemandRepository = positionDemandRepository;
        }
        public async Task<List<PotentialPositionListItemDto>> Handle(GetPotentialPositionsQuery request, CancellationToken cancellationToken)
        {
            List<PotentialPositionListItemDto> r = new List<PotentialPositionListItemDto>();
            var potentialPositions = await _potentialPositionRepository.GetAllAsync();
            potentialPositions = potentialPositions.OrderByDescending(x => x.Rating).ToList();
            var inProgressRequests = await _positionDemandRepository.GetInProgressDemands(PositionDemandSide.Open);
            foreach (var pp in potentialPositions)
            {
                if (inProgressRequests.Exists(x => x.Symbol == pp.Symbol))
                {
                    var vm = _mapper.Map<PotentialPositionListItemDto>(pp);
                    vm.InProgress = true;
                    r.Add(vm);
                }
            }

            foreach (var pp in potentialPositions)
            {
                if (r.Exists(x => x.Symbol == pp.Symbol))
                    continue;
                var vm = _mapper.Map<PotentialPositionListItemDto>(pp);
                vm.InProgress = false;
                r.Add(vm);
            }
            return r;
        }
    }
}

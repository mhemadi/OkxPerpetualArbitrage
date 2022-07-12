using AutoMapper;
using OkxPerpetualArbitrage.Application.Contracts.Logic;
using OkxPerpetualArbitrage.Application.Contracts.Persistance;
using OkxPerpetualArbitrage.Application.Models.DTOs;
using OkxPerpetualArbitrage.Domain.Entities.Enums;

namespace OkxPerpetualArbitrage.Application.Services
{
    public class GetPotentialPositionsLogic : IGetPotentialPositionsLogic
    {
        private readonly IMapper _mapper;
        private readonly IPotentialPositionRepository _potentialPositionRepository;
        private readonly IPositionDemandRepository _positionDemandRepository;

        public GetPotentialPositionsLogic(IMapper mapper, IPotentialPositionRepository potentialPositionRepository, IPositionDemandRepository positionDemandRepository)
        {
            _mapper = mapper;
            _potentialPositionRepository = potentialPositionRepository;
            _positionDemandRepository = positionDemandRepository;
        }
        public async Task<List<PotentialPositionListItemDto>> GetPotentialPositions(CancellationToken cancellationToken)
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

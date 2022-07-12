using OkxPerpetualArbitrage.Application.Contracts.Logic;
using OkxPerpetualArbitrage.Application.Contracts.Persistance;
using OkxPerpetualArbitrage.Domain.Entities.Enums;

namespace OkxPerpetualArbitrage.Application.Services
{
    public class InProgressDemandLogic : IInProgressDemandLogic
    {
        private readonly IPositionDemandRepository _positionDemandRepository;
        private readonly IPotentialPositionRepository _potentialPositionRepository;

        public InProgressDemandLogic(IPositionDemandRepository positionDemandRepository, IPotentialPositionRepository potentialPositionRepository)
        {
            _positionDemandRepository = positionDemandRepository;
            _potentialPositionRepository = potentialPositionRepository;
        }
        public async Task<decimal> GetTotalInProgressRequiredFunds()
        {
            decimal sumFund = 0;
            var inProgressDemands = await _positionDemandRepository.GetInProgressDemands(PositionDemandSide.Open);
            foreach (var demand in inProgressDemands)
            {
                var pp = await _potentialPositionRepository.GetPotentialPosition(demand.Symbol);
                sumFund += (demand.TotalSize * pp.MarkPrice);
            }
            return sumFund;
        }
    }
}

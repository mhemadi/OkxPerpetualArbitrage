using OkxPerpetualArbitrage.Application.Contracts.Logic;
using OkxPerpetualArbitrage.Application.Contracts.Persistance;
using OkxPerpetualArbitrage.Domain.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OkxPerpetualArbitrage.Application.Services
{
 

    public class TotalAvailableCloseSizeCalculator : ITotalAvailableCloseSizeCalculator
    {
        private readonly IPositionDemandRepository _positionDemandRepository;
        private readonly IOrderFillRepository _orderFillRepository;
        public TotalAvailableCloseSizeCalculator(IPositionDemandRepository positionDemandRepository, IOrderFillRepository orderFillRepository)
        {
            _positionDemandRepository = positionDemandRepository;
            _orderFillRepository = orderFillRepository;
        }

        public async Task<decimal> GetTotalAvailableSize(string symbol)
        {
            var openRequests = await _positionDemandRepository.GetIncompleteDemands(symbol);
            decimal avlSize = 0;
            foreach (var request in openRequests)
            {
                foreach (var fill in await _orderFillRepository.GetOrderFillsByPositionDemandId(request.PositionDemandId))
                {
                    if (fill.PartInPosition == PartInPosition.PerpOpenSell)
                        avlSize += (fill.Filled * fill.Lot);
                    if (fill.PartInPosition == PartInPosition.PerpCloseSell)
                        avlSize -= (fill.Filled * fill.Lot);
                }
            }
            return avlSize;
        }
    }

}

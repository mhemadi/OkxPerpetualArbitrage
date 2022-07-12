using Microsoft.Extensions.Logging;
using OkxPerpetualArbitrage.Application.Contracts.Logic;
using OkxPerpetualArbitrage.Application.Contracts.Persistance;
using OkxPerpetualArbitrage.Application.Models.OkexApi;
using OkxPerpetualArbitrage.Domain.Entities;
using OkxPerpetualArbitrage.Domain.Entities.Enums;

namespace OkxPerpetualArbitrage.Application.Services
{

    public class OrderFillCreateLogic : IOrderFillCreateLogic
    {
        private readonly IPositionDemandRepository _positionDemandRepository;
        private readonly IOrderFillRepository _orderFillRepository;
        private readonly ILogger<OrderFillCreateLogic> _logger;

        public OrderFillCreateLogic(IPositionDemandRepository positionDemandRepository, IOrderFillRepository orderFillRepository
            , ILogger<OrderFillCreateLogic> logger)
        {
            _positionDemandRepository = positionDemandRepository;
            _orderFillRepository = orderFillRepository;
            _logger = logger;
        }
        public async Task AddOrderFill(OKEXOrder oKEXOrder, int positionDemandId, PartInPosition partInPosition, PotentialPosition potentialPosition)
        {
            var demand = await _positionDemandRepository.GetPositionDemandNoTracking(positionDemandId);
            var fill = new OrderFill()
            {
                ClientOrderId = oKEXOrder.ClientOrderId,
                Fee = oKEXOrder.Fee,
                FeeCurrency = oKEXOrder.FeeCurrency,
                Filled = oKEXOrder.Size,
                PartInPosition = partInPosition,
                PositionDemand = demand,
                Price = oKEXOrder.Price,
                Size = oKEXOrder.InitialSize,
                Lot = 1,
                TimeStamp = oKEXOrder.CreatedDate
            };
            decimal filled = demand.Filled;
            if (partInPosition == PartInPosition.PerpCloseSell || partInPosition == PartInPosition.PerpOpenSell)
            {
                filled += oKEXOrder.Size * potentialPosition.ContractValuePerp;
                fill.Lot = potentialPosition.ContractValuePerp;
            }
            await _orderFillRepository.AddAsync(fill);
            if (filled != demand.Filled)
            {
                _logger.LogInformation("Setting filled to {filled} for demand {positionDemandId}", filled, positionDemandId);
                await _positionDemandRepository.SetFilled(positionDemandId, filled);
            }
        }
    }
}

using Microsoft.Extensions.Logging;
using OkxPerpetualArbitrage.Application.Contracts.Logic;
using OkxPerpetualArbitrage.Application.Contracts.OkxApi;
using OkxPerpetualArbitrage.Application.Contracts.Persistance;
using OkxPerpetualArbitrage.Domain.Entities;
using OkxPerpetualArbitrage.Domain.Entities.Enums;

namespace OkxPerpetualArbitrage.Application.Services
{
    public class PositionCheckLogic : IPositionCheckLogic
    {
        private readonly IOkxApiWrapper _apiWrapper;
        private readonly IPositionDemandRepository _positionDemandRepository;
        private readonly IOrderFillRepository _orderFillRepository;
        private readonly ILogger<PositionCheckLogic> _logger;

        public PositionCheckLogic(IOkxApiWrapper apiWrapper, IPositionDemandRepository positionDemandRepository, IOrderFillRepository orderFillRepository
            , ILogger<PositionCheckLogic> logger)
        {
            _apiWrapper = apiWrapper;
            _positionDemandRepository = positionDemandRepository;
            _orderFillRepository = orderFillRepository;
            _logger = logger;
        }
        public async Task Checkposition(string symbol, PotentialPosition pp)
        {
            var posSize = await _apiWrapper.GetPositionSize(symbol);
            posSize *= pp.ContractValuePerp;
            var spotBalance = await _apiWrapper.GetBalance(symbol);
            var cash = spotBalance == null ? 0 : spotBalance.Cash;
            var openDemands = await _positionDemandRepository.GetIncompleteDemands(symbol);
            var fills = new List<OrderFill>();
            foreach (var d in openDemands)
            {
                fills.AddRange(await _orderFillRepository.GetOrderFillsByPositionDemandId(d.PositionDemandId));
            }

            var spotSize = fills.Where(x => x.PartInPosition == PartInPosition.SpotBuy).Sum(x => x.Filled) + fills.Where(x => x.PartInPosition == PartInPosition.SpotBuy).Sum(x => x.Fee);
            spotSize -= fills.Where(x => x.PartInPosition == PartInPosition.SpotSell).Sum(x => x.Filled);
            var size = fills.Where(x => x.PartInPosition == PartInPosition.PerpOpenSell).Sum(x => x.Filled);
            size -= fills.Where(x => x.PartInPosition == PartInPosition.PerpCloseSell).Sum(x => x.Filled);
            size *= pp.ContractValuePerp;

            if (posSize != size)
            {
                _logger.LogCritical("sum of order fill size {sumSize} is not equal to position size {posSize}", size, posSize);
            }
            if (cash < spotSize)
            {
                _logger.LogCritical("available balance {cash} is not equal to sum spot size {spotSize}", cash, spotSize);
            }
            if (cash < posSize)
            {
                _logger.LogCritical("available balance {cash} is smaller than position size {posSize}", cash, posSize);
            }
        }
    }
}

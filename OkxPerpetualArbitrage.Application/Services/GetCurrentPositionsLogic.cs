using OkxPerpetualArbitrage.Application.Contracts.Logic;
using OkxPerpetualArbitrage.Application.Contracts.OkxApi;
using OkxPerpetualArbitrage.Application.Contracts.Persistance;
using OkxPerpetualArbitrage.Application.Models.DTOs;
using OkxPerpetualArbitrage.Domain.Entities;
using OkxPerpetualArbitrage.Domain.Entities.Enums;

namespace OkxPerpetualArbitrage.Application.Services
{
    public class GetCurrentPositionsLogic : IGetCurrentPositionsLogic
    {
        private readonly IPositionDemandRepository _positionDemandRepository;
        private readonly IOrderFillRepository _orderFillRepository;
        private readonly IPotentialPositionRepository _potentialPositionRepository;
        private readonly IOkxApiWrapper _apiWrapper;
        private readonly IFundingIncomeRepository _fundingIncomeRepository;

        public GetCurrentPositionsLogic(IPositionDemandRepository positionDemandRepository, IOrderFillRepository orderFillRepository, IPotentialPositionRepository potentialPositionRepository
            , IOkxApiWrapper apiWrapper, IFundingIncomeRepository fundingIncomeRepository)
        {
            _positionDemandRepository = positionDemandRepository;
            _orderFillRepository = orderFillRepository;
            _potentialPositionRepository = potentialPositionRepository;
            _apiWrapper = apiWrapper;
            _fundingIncomeRepository = fundingIncomeRepository;
        }
        public async Task<List<CurrentPositionListItemDto>> GetCurrentPositions(bool checkError, CancellationToken cancellationToken)
        {
            var r = new List<CurrentPositionListItemDto>();
            var symbolInfo = await _apiWrapper.GetAllSymbols();
            var openDemands = await _positionDemandRepository.GetIncompleteDemands();
            Dictionary<string, List<OrderFill>> dic = new Dictionary<string, List<OrderFill>>();
            foreach (var demand in openDemands)
            {
                if (demand.PositionDemandState == PositionDemandState.InProgress && demand.Filled == 0)
                    continue;
                if (!dic.ContainsKey(demand.Symbol))
                    dic.Add(demand.Symbol, new List<OrderFill>());
                foreach (var fill in await _orderFillRepository.GetOrderFillsByPositionDemandId(demand.PositionDemandId))
                {
                    dic[demand.Symbol].Add(fill);
                }
            }
            var inProgressDemands = await _positionDemandRepository.GetInProgressDemands(PositionDemandSide.Close);
            foreach (var d in dic)
            {
                var c = new CurrentPositionListItemDto();
                c.Symbol = d.Key;
                var pp = await _potentialPositionRepository.GetPotentialPosition(c.Symbol);
                decimal spotSize = d.Value.Where(x => x.PartInPosition == PartInPosition.SpotBuy).Sum(x => x.Filled) + d.Value.Where(x => x.PartInPosition == PartInPosition.SpotBuy).Sum(x => x.Fee);
                spotSize -= d.Value.Where(x => x.PartInPosition == PartInPosition.SpotSell).Sum(x => x.Filled);
                decimal size = d.Value.Where(x => x.PartInPosition == PartInPosition.PerpOpenSell).Sum(x => x.Filled);
                size -= d.Value.Where(x => x.PartInPosition == PartInPosition.PerpCloseSell).Sum(x => x.Filled);
                size *= pp.ContractValuePerp;
                c.PositionSize = size;
                c.PositionValue = size * pp.MarkPrice;
                c.CurrentFunding = pp.Funding * c.PositionValue / 100;
                c.NextFunding = pp.NextFunding * c.PositionValue / 100;
                c.TotalFees = d.Value.Where(x => x.FeeCurrency == "USDT").Sum(x => x.Fee);
                c.TotalFundingIncome = await _fundingIncomeRepository.GetPositionFunding(c.Symbol);
                c.OpenDate = d.Value.FirstOrDefault().TimeStamp;
                var symInfo = symbolInfo.Where(x => x.Instrument == c.Symbol).FirstOrDefault();
                decimal buyPriceSpot = 0;
                decimal sellPriceSpot = 0;
                decimal buyPricePerp = 0;
                decimal sellPricePerp = 0;
                foreach (var v in d.Value.Where(x => x.PartInPosition == PartInPosition.SpotBuy))
                    buyPriceSpot += (v.Price * (v.Filled + v.Fee));
                foreach (var v in d.Value.Where(x => x.PartInPosition == PartInPosition.SpotSell))
                    sellPriceSpot += (v.Price * v.Filled);
                foreach (var v in d.Value.Where(x => x.PartInPosition == PartInPosition.PerpCloseSell))
                    buyPricePerp += (v.Price * v.Filled * pp.ContractValuePerp);
                foreach (var v in d.Value.Where(x => x.PartInPosition == PartInPosition.PerpOpenSell))
                    sellPricePerp += (v.Price * v.Filled * pp.ContractValuePerp);

                var remainingSize = size - (d.Value.Where(x => x.PartInPosition == PartInPosition.PerpCloseSell).Sum(x => x.Filled) * pp.ContractValuePerp);//Check if perp and spot size matches
                buyPricePerp += (symInfo.PerpAsk * remainingSize);
                sellPriceSpot += (symInfo.SpotBid * remainingSize);

                if (spotSize < size)
                    spotSize = size;

                if (size != 0 && spotSize != 0)
                {
                    buyPriceSpot /= spotSize;
                    sellPriceSpot /= size;
                    buyPricePerp /= size;
                    sellPricePerp /= size;
                }

                var closeFee = c.TotalFees;//This is just an estimation. Can make more accurate guess
                var tmpPnl = (sellPriceSpot - buyPriceSpot) * size;
                tmpPnl += (sellPricePerp - buyPricePerp) * size;
                tmpPnl += closeFee;
                c.EstimatedCloseCost = tmpPnl;
                c.PNL = tmpPnl + c.TotalFees + c.TotalFundingIncome;

                c.CloseInProgress = false;
                if (inProgressDemands.Exists(x => x.Symbol == c.Symbol))
                    c.CloseInProgress = true;
                c.Error = "";
                if (checkError)
                {
                    var posSize = await _apiWrapper.GetPositionSize(c.Symbol);
                    var spotBalance = await _apiWrapper.GetBalance(c.Symbol);
                    spotSize = spotBalance == null ? 0 : spotBalance.Cash;
                    if (posSize != c.PositionSize / pp.ContractValuePerp)
                        c.Error += "Size doesnt match the position size on okex. ";
                    if (spotSize < c.PositionSize)
                        c.Error += "Size is larger than balance on okex. ";
                    if (!string.IsNullOrEmpty(c.Error))
                        c.Error += "Please manually check the db and the trades on okex and apply changes";
                }
                r.Add(c);
            }
            return r;
        }
    }
}

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OkxPerpetualArbitrage.Application.Contracts.ApiService;
using OkxPerpetualArbitrage.Application.Contracts.Logic;
using OkxPerpetualArbitrage.Application.Contracts.Persistance;
using OkxPerpetualArbitrage.Application.Models.InfrastructureSettings;
using OkxPerpetualArbitrage.Domain.Entities;
using OkxPerpetualArbitrage.Domain.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OkxPerpetualArbitrage.Application.Services
{
  

    public class PositionOpenLogic : IPositionOpenLogic
    {
        private readonly IPositionDemandRepository _positionDemandRepository;
        private readonly IOrderFillCreateLogic _orderFillCreateLogic;
        private readonly GeneralSetting _setting;
        private readonly ILogger<PositionOpenLogic> _logger;
        private readonly IApiService _apiService;
        private readonly IFundingIncomeRepository _fundingIncomeRepository;
        private readonly IPositionHistoryRepository _positionHistoryRepository;
        private readonly IPositionChunkCreateLogic _positionChunkCreateLogic;
        private readonly IOrderFillRepository _orderFillRepository;
        private readonly IPotentialPositionRepository _potentialPositionRepository;
        private readonly IPositionCheckLogic _positionCheckLogic;

        public PositionOpenLogic(IPositionDemandRepository positionDemandRepository, IOrderFillCreateLogic orderFillCreateLogic, IOptions<GeneralSetting> setting
            , ILogger<PositionOpenLogic> logger, IApiService apiService, IFundingIncomeRepository fundingIncomeRepository, IPositionHistoryRepository positionHistoryRepository
            , IPositionChunkCreateLogic positionChunkCreateLogic, IOrderFillRepository orderFillRepository, IPotentialPositionRepository potentialPositionRepository
            , IPositionCheckLogic positionCheckLogic)
        {
            _positionDemandRepository = positionDemandRepository;
            _orderFillCreateLogic = orderFillCreateLogic;
            _setting = setting.Value;
            _logger = logger;
            _apiService = apiService;
            _fundingIncomeRepository = fundingIncomeRepository;
            _positionHistoryRepository = positionHistoryRepository;
            _positionChunkCreateLogic = positionChunkCreateLogic;
            _orderFillRepository = orderFillRepository;
            _potentialPositionRepository = potentialPositionRepository;
            _positionCheckLogic = positionCheckLogic;
        }

        public async Task Open(string symbol, int positionDemandId, decimal lotSize, decimal lotSizeChunk, decimal minSpread, PotentialPosition potentialPosition)
        {
            var demand = await _positionDemandRepository.GetPositionDemandNoTracking(positionDemandId);
            if (demand == null)
            {
                _logger.LogError("Could not find the demand to open the position {symbol} {positionDemandId} ", symbol, positionDemandId);
                throw new Exception("Could not find the demand to open the position");
            }
            try
            {
                int tries = 0;
                while (lotSize != 0 && !demand.IsCanceled && tries < _setting.MaxOpenTries)
                {
                    _logger.LogInformation("startining try {tries} to open chunksize {lotSizeChunk} for demand {positionDemandId}", tries, lotSizeChunk, positionDemandId);
                    var filled = await _positionChunkCreateLogic.OpenClosePositionChunck(symbol, lotSizeChunk, positionDemandId, minSpread, potentialPosition, _setting.TryToBeMaker, true);
                    _logger.LogInformation("filled {filled} for demand {reqId}", filled, positionDemandId);
                    tries++;
                    lotSize -= filled;
                    if (lotSizeChunk > lotSize)
                        lotSizeChunk = lotSize;
                    demand = await _positionDemandRepository.GetPositionDemandNoTracking(positionDemandId);
                    _logger.LogInformation("ended loop  for demand {positionDemandId} cancel {IsCanceled}", positionDemandId, demand.IsCanceled);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed opening position {symbol} {positionDemandId} ", symbol, positionDemandId);
            }
            finally
            {
                _logger.LogInformation("Updating the initial open demand {positionDemandId}", positionDemandId);
                var tmp = await _positionDemandRepository.GetIncompleteDemandsNoTracking(symbol);
                demand = await _positionDemandRepository.GetPositionDemandNoTracking(positionDemandId);
                var demandTracked = await _positionDemandRepository.GetByIdAsync(positionDemandId);
                demandTracked.Filled = demand.Filled;
                demandTracked.IsCanceled = demand.IsCanceled;
                demandTracked.PositionDemandState = PositionDemandState.InPosition;
                if (demandTracked.Filled == 0)
                    demandTracked.PositionDemandState = PositionDemandState.Done;


                var pp = await _potentialPositionRepository.GetPotentialPosition(symbol);
                var fills = await _orderFillRepository.GetOrderFillsByPositionDemandId(positionDemandId);
                decimal spotSize = fills.Where(x => x.PartInPosition == PartInPosition.SpotBuy).Sum(x => x.Filled) + fills.Where(x => x.PartInPosition == PartInPosition.SpotBuy).Sum(x => x.Fee);
                decimal size = fills.Where(x => x.PartInPosition == PartInPosition.PerpOpenSell).Sum(x => x.Filled);
                size *= pp.ContractValuePerp;
                //Update the actual spread if its useful
                if (demandTracked.Filled != 0)
                {
                    decimal buyPriceSpot = 0;
                    decimal sellPricePerp = 0;
                    foreach (var v in fills.Where(x => x.PartInPosition == PartInPosition.SpotBuy))
                        buyPriceSpot += (v.Price * (v.Filled + v.Fee));

                    foreach (var v in fills.Where(x => x.PartInPosition == PartInPosition.PerpOpenSell))
                        sellPricePerp += (v.Price * v.Filled * pp.ContractValuePerp);

                    if (spotSize > 0 && size > 0)
                    {
                        buyPriceSpot /= spotSize;
                        sellPricePerp /= size;
                        var spread = (sellPricePerp - buyPriceSpot) / ((sellPricePerp + buyPriceSpot) / 2) * 100;

                        demandTracked.ActualSpread = spread;
                    }
                    else
                        _logger.LogError("Demand {demandId} has filled at {filled} but spot size {spotSize} and perp size {perpsize}", positionDemandId, demandTracked.Filled, spotSize, size);
                }
                demandTracked.UpdateDate = DateTime.UtcNow;
                await _positionDemandRepository.UpdateAsync(demandTracked);


                demand = await _positionDemandRepository.GetPositionDemandNoTracking(positionDemandId);









                if (size != demand.Filled)
                {
                    _logger.LogCritical("sum of order fill size {sumSize} is not equal to demand fill size {filled}", size, demand.Filled);
                    demandTracked.Filled = size;
                    await _positionDemandRepository.UpdateAsync(demandTracked);
                }
                if (spotSize < size)
                {
                    _logger.LogCritical("sum of order fill size {sumSize} is larger than spot size {spotSize}", size, spotSize);
                }



                await _positionCheckLogic.Checkposition(symbol, pp);


            }
        }
    }
}

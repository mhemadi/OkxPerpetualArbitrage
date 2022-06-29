using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OkxPerpetualArbitrage.Application.Contracts.OkxApi;
using OkxPerpetualArbitrage.Application.Contracts.BackgroundService;
using OkxPerpetualArbitrage.Application.Contracts.Persistance;
using OkxPerpetualArbitrage.Application.Helpers;
using OkxPerpetualArbitrage.Domain.Entities;

namespace OkxPerpetualArbitrage.Application.Services.BackgroundTaskServices
{
    /// <summary>
    /// This service keeps an uptodate version of data from the exchange regarding potential positions, in the db
    /// The data is used as a cache to prevent multiple calls to the exchage api 
    /// </summary>

    public class PotentialPositionUpdater : IPotentialPositionUpdater
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<PotentialPositionUpdater> _logger;
        private int counter = 0;
        private bool reset = false;

        public PotentialPositionUpdater(IServiceProvider serviceProvider, ILogger<PotentialPositionUpdater> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }
        public async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        if (counter == 500)
                        {
                            reset = true;
                            counter = 0;
                            _logger.LogInformation("Gonna reset all the records in potential positions");
                        }
                        else
                            reset = false;

                        counter++;
                        var _apiService = scope.ServiceProvider.GetRequiredService<IOkxApiWrapper>();
                        var _potentialPositionRep = scope.ServiceProvider.GetRequiredService<IPotentialPositionRepository>();
                        var positionDemandRepository = scope.ServiceProvider.GetRequiredService<IPositionDemandRepository>();
                        var ratingReposirtory = scope.ServiceProvider.GetRequiredService<IPotentialPositionRatingHistoryRepository>();

                        await SavePotentialPositions(_apiService, _potentialPositionRep, ratingReposirtory);
                    }

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed updating pp");
                }
                _logger.LogInformation("Saved PPs");
                await Task.Delay(1 * 1 * 1000, stoppingToken);

            }
        }


        public async Task SavePotentialPositions(IOkxApiWrapper _apiService, IPotentialPositionRepository _potentialPositionRep, IPotentialPositionRatingHistoryRepository potentialPositionRatingHistoryRepository)
        {
            List<decimal> ratings = new List<decimal>();
            var symbols = await _apiService.GetAllSymbols(2, 1000);
            if (symbols == null || symbols.Count == 0)
            {
                _logger.LogError("Failed to save potential positions");
                return;
            }


            var all = await _potentialPositionRep.GetAllAsync();



            foreach (var symbol in symbols)
            {
                await _potentialPositionRep.UnUpToDate(symbol.Instrument);
                var fundings = await _apiService.GetFundingRates(symbol.Instrument, 5, 100);
                var spread = (symbol.PerpBid - symbol.SpotAsk) / ((symbol.PerpBid + symbol.SpotAsk) / 2);
                var closeSpread = (symbol.SpotBid - symbol.PerpAsk) / ((symbol.PerpAsk + symbol.SpotBid) / 2);
                var rateHistory = await _apiService.GetFundingHistory(symbol.Instrument, 3,1000);


                var ratingAvg3Days = rateHistory.Average();
                var ratingAvg7Days = rateHistory.Average();
                var ratingAvg14Days = rateHistory.Average();
                if (rateHistory.Count > 9)
                    ratingAvg3Days = rateHistory.GetRange(0, 9).Average();
                if (rateHistory.Count > 21)
                    ratingAvg7Days = rateHistory.GetRange(0, 21).Average();


                var nf = (fundings.Item2 * 1.5m);
                if (nf > 0)
                    nf *= (1 - (DateTimeHelper.TimeToFunding().Hours / 8m));
                var cf = (fundings.Item1 * 1.5m);
                spread /= 3;
                if (spread > 0)
                    spread /= 2;

                if ((nf > 0 && cf > 0) || (nf < 0 && cf < 0))
                {
                    nf *= 2m;
                    cf *= 2m;
                }
                if (spread < 0)
                    spread *= 4m;
                if (spread < -0.05m)
                    spread *= 4m;
                if (spread < -0.1m)
                    spread *= 4m;

                //    var rateHistory = await _apiService.GetFundingHistory(symbol.Instrument);
                var avgRate = ratingAvg3Days + ratingAvg14Days + ratingAvg7Days;
                if ((ratingAvg3Days > 0 && ratingAvg14Days > 0 && ratingAvg7Days > 0) || (ratingAvg3Days < 0 && ratingAvg14Days < 0 && ratingAvg7Days < 0))
                    avgRate *= 2;

                var rating = (spread) + nf + cf + avgRate;




                var pp = all.Where(x => x.Symbol == symbol.Instrument).FirstOrDefault();



                if (pp != null)
                {
                    pp.Funding = Math.Round(fundings.Item1 * 100, 4);
                    pp.IsUpToDate = true;
                    pp.NextFunding = Math.Round(fundings.Item2 * 100, 4);
                    pp.Rating = Math.Round(rating * 10000, 2);
                    pp.Spread = Math.Round(spread * 100, 4);
                    pp.CloseSpread = Math.Round(closeSpread * 100, 4);
                    pp.MarkPrice = Math.Round((symbol.PerpBid + symbol.SpotAsk) / 2, 10);
                    pp.FundingRateAverageThreeDays = ratingAvg3Days;
                    pp.FundingRateAverageSevenDays = ratingAvg7Days;
                    pp.FundingRateAverageFourteenDays = ratingAvg14Days;
                    if (reset)
                    {
                        var perpContract = await _apiService.GetPerpContractData(symbol.Instrument, 2, 1000);
                        await Task.Delay(100);
                        var spotContract = await _apiService.GetSpotContractData(symbol.Instrument, 2, 1000);
                        await Task.Delay(100);
                        var fees = await _apiService.GetSpotAndPerpMakerAndTakerFees(symbol.Instrument, 2, 1000);
                        await Task.Delay(100);
                        if (perpContract == null || spotContract == null || fees == null)
                        {
                            _logger.LogError("Failed updating potential position for {symbol}", symbol.Instrument);
                            continue;
                        }
                        pp.ContractValuePerp = perpContract.Item3;
                        pp.FeePerpMaker = fees.Item3;
                        pp.FeePerpTaker = fees.Item4;
                        pp.FeeSpotMaker = fees.Item1;
                        pp.FeeSpotTaker = fees.Item2;
                        pp.LotSizeSpot = spotContract.Item2;
                        pp.MinSizePerp = perpContract.Item1;
                        pp.MinSizeSpot = spotContract.Item1;
                        pp.TickSizePerp = perpContract.Item2;
                        pp.TickSizeSpot = spotContract.Item3;
                    }
                    await _potentialPositionRep.UpdateAsync(pp);
                }
                else
                {
                    var perpContract = await _apiService.GetPerpContractData(symbol.Instrument, 2, 1000);
                    await Task.Delay(100);
                    var spotContract = await _apiService.GetSpotContractData(symbol.Instrument, 2, 1000);
                    await Task.Delay(100);
                    var fees = await _apiService.GetSpotAndPerpMakerAndTakerFees(symbol.Instrument, 2, 1000);
                    await Task.Delay(100);
                    if (perpContract == null || spotContract == null || fees == null)
                    {
                        _logger.LogError("Failed creating potential position for {symbol}", symbol.Instrument);
                        continue;
                    }
                    var ppNew = new PotentialPosition()
                    {
                        ContractValuePerp = perpContract.Item3,
                        FeePerpMaker = fees.Item3,
                        FeePerpTaker = fees.Item4,
                        FeeSpotMaker = fees.Item1,
                        FeeSpotTaker = fees.Item2,
                        Funding = Math.Round(fundings.Item1 * 100, 4),
                        IsUpToDate = true,
                        LotSizeSpot = spotContract.Item2,
                        MinSizePerp = perpContract.Item1,
                        MinSizeSpot = spotContract.Item1,
                        NextFunding = Math.Round(fundings.Item2 * 100, 4),
                        Rating = Math.Round(rating * 10000, 2),
                        Spread = Math.Round(spread * 100, 4),
                        CloseSpread = Math.Round(closeSpread * 100, 4),
                        Symbol = symbol.Instrument,
                        TickSizePerp = perpContract.Item2,
                        TickSizeSpot = spotContract.Item3,
                        FundingRateAverageThreeDays = ratingAvg3Days,
                        FundingRateAverageSevenDays = ratingAvg7Days,
                        FundingRateAverageFourteenDays = ratingAvg14Days,
                        MarkPrice = Math.Round((symbol.PerpBid + symbol.SpotAsk) / 2, 10)
                    };
                    await _potentialPositionRep.AddAsync(ppNew);



                }
            }

            //all = await _potentialPositionRep.GetAllAsync();

            //var sorted = all.OrderByDescending(x => x.Rating).ToList();
            //if (sorted.Count > 1)
            //{
            //    await potentialPositionRatingHistoryRepository.AddAsync(new PotentialPositionRatingHistory() { Rating = sorted[0].Rating, Symbol = sorted[0].Symbol, Timestamp = DateTime.UtcNow });
            //    await potentialPositionRatingHistoryRepository.AddAsync(new PotentialPositionRatingHistory() { Rating = sorted[1].Rating, Symbol = sorted[1].Symbol, Timestamp = DateTime.UtcNow });
            //}


        }



    }
}


using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OkxPerpetualArbitrage.Application.Contracts.ApiService;
using OkxPerpetualArbitrage.Application.Contracts.BackgroundService;
using OkxPerpetualArbitrage.Application.Contracts.Persistance;
using OkxPerpetualArbitrage.Application.Helpers;
using OkxPerpetualArbitrage.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OkxPerpetualArbitrage.Application.Services.BackgroundTaskServices
{


    public class FundsUpdater :  IFundsUpdater
    {
        private readonly ILogger<FundsUpdater> _logger;
        private IApiService _apiService;
        private readonly IServiceProvider _serviceProvider;
        private IFundingIncomeRepository _fundingIncomeRepository;

        public FundsUpdater(ILogger<FundsUpdater> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
          
        }
        public async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        _apiService = scope.ServiceProvider.GetRequiredService<IApiService>();
                        _fundingIncomeRepository = scope.ServiceProvider.GetRequiredService<IFundingIncomeRepository>();
                        await SaveFundingBills(_apiService, _fundingIncomeRepository);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError("Failed updating pp " + ex.Message + ex.StackTrace.ToString());
                }
                int waitMiliSec = Convert.ToInt32(DateTimeHelper.TimeToFunding().TotalMilliseconds + (1000 * 60 * 10));
                _logger.LogInformation($"Saved Funding, waiting {waitMiliSec / 60000} minutues");
                await Task.Delay(waitMiliSec, stoppingToken);
            }
        }
      

        public async Task SaveFundingBills(IApiService _apiService, IFundingIncomeRepository _fundingRep)
        {
            var bills = await _apiService.GetRecentFundingBills();
            List<FundingIncome> incomes = new List<FundingIncome>();
            foreach (var bill in bills)
            {
                FundingIncome income = new FundingIncome()
                {
                    BillId = bill.BillId,
                    IsInCurrentPosition = true,
                    TimeStamp = bill.TimeStamp,
                    Amount = bill.BalanceChange,
                    Symbol = bill.Instrument.Split('-')[0]
                };
                incomes.Add(income);
            }
            await _fundingRep.AddAll(incomes);
        }


    }
}

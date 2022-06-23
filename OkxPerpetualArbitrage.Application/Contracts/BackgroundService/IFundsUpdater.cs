using OkxPerpetualArbitrage.Application.Contracts.ApiService;
using OkxPerpetualArbitrage.Application.Contracts.Persistance;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OkxPerpetualArbitrage.Application.Contracts.BackgroundService
{
    public interface IFundsUpdater : IBackgroundServiceTask
    {
        Task SaveFundingBills(IApiService _apiService, IFundingIncomeRepository _fundingRep);
    }
}

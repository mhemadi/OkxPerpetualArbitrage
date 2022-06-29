using OkxPerpetualArbitrage.Application.Contracts.OkxApi;
using OkxPerpetualArbitrage.Application.Contracts.Persistance;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OkxPerpetualArbitrage.Application.Contracts.BackgroundService
{
    public interface IFundsUpdater : IBackgroundServiceTask
    {
        Task SaveFundingBills(IOkxApiWrapper _apiService, IFundingIncomeRepository _fundingRep);
    }
}

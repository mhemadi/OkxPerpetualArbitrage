using OkxPerpetualArbitrage.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OkxPerpetualArbitrage.Application.Contracts.Persistance
{
    public interface IFundingIncomeRepository : IAsyncRepository<FundingIncome>
    {
        Task AddAll(List<FundingIncome> fundingIncomes);
        Task<decimal> GetPositionFunding(string symbol);
        Task EndCurrentPositionStatus(string symbol);
        Task DeleteInPosition(string symbol);

    }
}

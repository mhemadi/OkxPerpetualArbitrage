using OkxPerpetualArbitrage.Domain.Entities;

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

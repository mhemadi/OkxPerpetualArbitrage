using Microsoft.EntityFrameworkCore;
using OkxPerpetualArbitrage.Application.Contracts.Persistance;
using OkxPerpetualArbitrage.Domain.Entities;

namespace OkxPerpetualArbitrage.Persistance.Repositories
{
    public class FundingIncomeRepository : AsyncRepository<FundingIncome>, IFundingIncomeRepository
    {
        public FundingIncomeRepository(SpotPerpDbContext spotPerpDbContext) : base(spotPerpDbContext)
        {
        }
        public async Task AddAll(List<FundingIncome> fundingIncomes)
        {
            var currentFundings = _dbContext.FundingIncomes.Where(x => x.TimeStamp > DateTime.UtcNow.AddDays(-1)).ToList();
            foreach (var f in fundingIncomes)
            {
                if (!currentFundings.Exists(x => x.BillId == f.BillId))
                    _dbContext.Add(f);
            }
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteInPosition(string symbol)
        {
            var list = _dbContext.FundingIncomes.Where(x => x.IsInCurrentPosition && x.Symbol == symbol);
            _dbContext.FundingIncomes.RemoveRange(list);
            await _dbContext.SaveChangesAsync();
        }

        public async Task EndCurrentPositionStatus(string symbol)
        {
            foreach (var f in _dbContext.FundingIncomes.Where(x => x.IsInCurrentPosition && x.Symbol == symbol))
            {
                f.IsInCurrentPosition = false;
                _dbContext.FundingIncomes.Update(f);
            }
            await _dbContext.SaveChangesAsync();
        }

        public async Task<decimal> GetPositionFunding(string symbol)
        {
            var list = await _dbContext.FundingIncomes.Where(x => x.IsInCurrentPosition && x.Symbol == symbol).ToListAsync();
            return list.Sum(x => x.Amount);
        }
    }
}

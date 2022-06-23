using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using OkxPerpetualArbitrage.Application.Contracts.Persistance;
using OkxPerpetualArbitrage.Domain.Entities;
using OkxPerpetualArbitrage.Domain.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OkxPerpetualArbitrage.Persistance.Repositories
{
    class PositionDemandRepository : AsyncRepository<PositionDemand>, IPositionDemandRepository
    {
        private readonly object updateLock = new object();
        public PositionDemandRepository(SpotPerpDbContext spotPerpDbContext) : base(spotPerpDbContext)
        {
        }

        public async Task<List<PositionDemand>> GetIncompleteDemands(string symbol)
        {
            return await _dbContext.PositionDemands.Where(x => x.Symbol == symbol && x.PositionDemandState != PositionDemandState.Done).ToListAsync();
        }

        public async Task<List<PositionDemand>> GetIncompleteDemands()
        {
            return await _dbContext.PositionDemands.Where(x => x.PositionDemandState != PositionDemandState.Done).ToListAsync();
        }
        public async Task<List<PositionDemand>> GetUnfilledDemands()
        {
            return await _dbContext.PositionDemands.Where(x => x.PositionDemandState == PositionDemandState.Done && x.Filled == 0).ToListAsync();
        }

        public async Task<List<PositionDemand>> GetInProgressDemands(PositionDemandSide side)
        {
            return await _dbContext.PositionDemands.Where(x => x.PositionDemandState == PositionDemandState.InProgress && x.PositionDemandSide == side).ToListAsync();
        }

        public async Task<List<PositionDemand>> GetInProgressDemandsBySymbolAndSide(string symbol, PositionDemandSide side)
        {
            return await _dbContext.PositionDemands.Where(x => x.PositionDemandState == PositionDemandState.InProgress && x.Symbol == symbol && x.PositionDemandSide == side).ToListAsync();
        }

        public async Task<List<PositionDemand>> GetInProgressDemandsBySymbol(string symbol)
        {
            return await _dbContext.PositionDemands.Where(x => x.PositionDemandState == PositionDemandState.InProgress && x.Symbol == symbol).ToListAsync();
        }

        public async Task<bool> IsPositionDemandInProgress(string symbol)
        {
            return await _dbContext.PositionDemands.Where(x => x.Symbol == symbol && x.PositionDemandState == PositionDemandState.InProgress && !x.IsCanceled).CountAsync() > 0;
        }

        public async Task<bool> IsPositionDemandInProgress(string symbol, int excludeId)
        {
            return await _dbContext.PositionDemands.Where(x => x.Symbol == symbol && x.PositionDemandId != excludeId && x.PositionDemandState == PositionDemandState.InProgress && !x.IsCanceled).CountAsync() > 0;
        }

        public async Task<int> SetIsCanceled(int positionDemandId, bool isCanceled)
        {
            var commandText = "Update PositionDemands Set IsCanceled = (@IsCanceled), UpdateDate = (@UpdateDate) Where PositionDemandId = (@PositionDemandId)";
            var isCanceledParameter = new SqliteParameter("@IsCanceled", isCanceled);
            var updateDateParameter = new SqliteParameter("@UpdateDate", DateTime.UtcNow);
            var positionDemandIdParameter = new SqliteParameter("@PositionDemandId", positionDemandId);
            return await _dbContext.Database.ExecuteSqlRawAsync(commandText, isCanceledParameter, updateDateParameter, positionDemandIdParameter);
        }

        public async Task<int> SetFilled(int positionDemandId, decimal filled)
        {
            var commandText = "Update PositionDemands Set Filled = (@Filled), UpdateDate = (@UpdateDate) Where PositionDemandId = (@PositionDemandId)";
            var filledParameter = new SqliteParameter("@Filled", filled);
            var updateDateParameter = new SqliteParameter("@UpdateDate", DateTime.UtcNow);
            var positionDemandIdParameter = new SqliteParameter("@PositionDemandId", positionDemandId);
            return await _dbContext.Database.ExecuteSqlRawAsync(commandText, filledParameter, updateDateParameter, positionDemandIdParameter);
        }

        public async Task<PositionDemand> GetPositionDemandNoTracking(int positionDemandId)
        {
            var positionDemandIdParameter = new SqliteParameter("@PositionDemandId", positionDemandId);

            var positionDemand = await GetByIdAsync(positionDemandId);

            //var positionDemand = await _dbContext.PositionDemands.FromSqlRaw("Select * From PositionDemands Where PositionDemandId = (@PositionDemandId)", positionDemandIdParameter).FirstOrDefaultAsync();
            await _dbContext.Entry(positionDemand).ReloadAsync();
            return positionDemand;
        }

        public async Task<List<PositionDemand>> GetIncompleteDemandsNoTracking(string symbol)
        {
            var symbolParameter = new SqliteParameter("@Symbol", symbol);
            var stateParameter = new SqliteParameter("@PositionDemandState", PositionDemandState.Done);
            var positionDemands = await GetIncompleteDemands(symbol);
            foreach (var entity in positionDemands)
            {
              _dbContext.Entry(entity).State = EntityState.Detached;
            }
        //  positionDemands = await GetIncompleteDemands(symbol);

            return positionDemands;
        }

        //public static void Reload(this CollectionEntry source)
        //{
        //    if (source.CurrentValue != null)
        //    {
        //        foreach (var item in source.CurrentValue)
        //            source.EntityEntry.Context.Entry(item).State = EntityState.Detached;
        //        source.CurrentValue = null;
        //    }
        //    source.IsLoaded = false;
        //    source.Load();
        //}
    }
}

using Microsoft.EntityFrameworkCore;
using OkxPerpetualArbitrage.Domain.Entities;

namespace OkxPerpetualArbitrage.Persistance
{
    public class SpotPerpDbContext : DbContext
    {
        public SpotPerpDbContext(DbContextOptions<SpotPerpDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(SpotPerpDbContext).Assembly);
        }

        public DbSet<PotentialPosition> PotentialPositions { get; set; }
        public DbSet<PositionDemand> PositionDemands { get; set; }
        public DbSet<FundingIncome> FundingIncomes { get; set; }
        public DbSet<OrderFill> OrderFills { get; set; }
    }
}

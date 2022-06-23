using Microsoft.EntityFrameworkCore;
using OkxPerpetualArbitrage.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace OkxPerpetualArbitrage.Persistance
{
    public class SpotPerpDbContext : DbContext
    {
        //  protected override void OnConfiguring(DbContextOptionsBuilder options)=> options.UseSqlite(@"Data Source=C:\sqliteStudio\SPA.db");
        public SpotPerpDbContext(DbContextOptions<SpotPerpDbContext> options) : base(options){}

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

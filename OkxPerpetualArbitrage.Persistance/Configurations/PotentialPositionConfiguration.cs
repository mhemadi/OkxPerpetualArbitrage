using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OkxPerpetualArbitrage.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace OkxPerpetualArbitrage.Persistance.Configurations
{
    public class PotentialPositionConfiguration : IEntityTypeConfiguration<PotentialPosition>
    {
        public void Configure(EntityTypeBuilder<PotentialPosition> entity)
        {
            entity.HasKey(e => e.PotentialPositionId);
            entity.Property(e => e.Symbol).IsRequired().HasColumnType(SqlDataTypes.ShortString);
            entity.Property(e => e.Funding).IsRequired().HasColumnType(SqlDataTypes.Instrument);
            entity.Property(e => e.NextFunding).IsRequired().HasColumnType(SqlDataTypes.Instrument);
            entity.Property(e => e.Spread).IsRequired().HasColumnType(SqlDataTypes.Instrument);
            entity.Property(e => e.CloseSpread).IsRequired().HasColumnType(SqlDataTypes.Instrument);
            entity.Property(e => e.Rating).IsRequired().HasColumnType(SqlDataTypes.Instrument);
            entity.Property(e => e.MinSizeSpot).IsRequired().HasColumnType(SqlDataTypes.Instrument);
            entity.Property(e => e.LotSizeSpot).IsRequired().HasColumnType(SqlDataTypes.Instrument);
            entity.Property(e => e.TickSizeSpot).IsRequired().HasColumnType(SqlDataTypes.Instrument);
            entity.Property(e => e.MinSizePerp).IsRequired().HasColumnType(SqlDataTypes.Instrument);
            entity.Property(e => e.TickSizePerp).IsRequired().HasColumnType(SqlDataTypes.Instrument);
            entity.Property(e => e.ContractValuePerp).IsRequired().HasColumnType(SqlDataTypes.Instrument);
            entity.Property(e => e.FeeSpotMaker).IsRequired().HasColumnType(SqlDataTypes.Instrument);
            entity.Property(e => e.FeeSpotTaker).IsRequired().HasColumnType(SqlDataTypes.Instrument);
            entity.Property(e => e.FeePerpMaker).IsRequired().HasColumnType(SqlDataTypes.Instrument);
            entity.Property(e => e.FeePerpTaker).IsRequired().HasColumnType(SqlDataTypes.Instrument);
            entity.Property(e => e.MarkPrice).IsRequired().HasColumnType(SqlDataTypes.Instrument);
            entity.Property(e => e.FundingRateAverageThreeDays).IsRequired().HasColumnType(SqlDataTypes.Instrument);
            entity.Property(e => e.FundingRateAverageSevenDays).IsRequired().HasColumnType(SqlDataTypes.Instrument);
            entity.Property(e => e.FundingRateAverageFourteenDays).IsRequired().HasColumnType(SqlDataTypes.Instrument);
            entity.Property(e => e.IsUpToDate).IsRequired();
        }
    }
}

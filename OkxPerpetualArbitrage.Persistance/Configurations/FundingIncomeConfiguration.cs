using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OkxPerpetualArbitrage.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace OkxPerpetualArbitrage.Persistance.Configurations
{
    public class FundingIncomeConfiguration : IEntityTypeConfiguration<FundingIncome>
    {
        public void Configure(EntityTypeBuilder<FundingIncome> entity)
        {
            entity.HasKey(e => e.FundingIncomeId);
            entity.Property(e => e.TimeStamp).HasColumnType(SqlDataTypes.Date);
            entity.Property(e => e.Amount).IsRequired().HasColumnType(SqlDataTypes.Instrument);
            entity.Property(e => e.BillId).IsRequired().HasColumnType(SqlDataTypes.MediumString);
            entity.Property(e => e.Symbol).IsRequired().HasColumnType(SqlDataTypes.ShortString);
            entity.Property(e => e.IsInCurrentPosition).IsRequired();
        }
    }
}

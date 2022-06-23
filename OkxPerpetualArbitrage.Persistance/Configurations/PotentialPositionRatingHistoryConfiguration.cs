using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OkxPerpetualArbitrage.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace OkxPerpetualArbitrage.Persistance.Configurations
{
    class PotentialPositionRatingHistoryConfiguration : IEntityTypeConfiguration<PotentialPositionRatingHistory>
    {
        public void Configure(EntityTypeBuilder<PotentialPositionRatingHistory> entity)
        {
            entity.HasKey(x => x.PotentialPositionRatingHistoryId);
            entity.Property(e => e.Symbol).IsRequired().HasColumnType(SqlDataTypes.ShortString);
            entity.Property(e => e.Rating).IsRequired().HasColumnType(SqlDataTypes.Instrument);
            entity.Property(e => e.Timestamp).HasColumnType(SqlDataTypes.Date);
        }
    }
}

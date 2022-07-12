using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OkxPerpetualArbitrage.Domain.Entities;

namespace OkxPerpetualArbitrage.Persistance.Configurations
{
    class PositionDemandConfiguration : IEntityTypeConfiguration<PositionDemand>
    {
        public void Configure(EntityTypeBuilder<PositionDemand> entity)
        {
            entity.HasKey(e => e.PositionDemandId);
            entity.Property(e => e.Symbol).IsRequired().HasColumnType(SqlDataTypes.ShortString);
            entity.Property(e => e.TotalSize).IsRequired().HasColumnType(SqlDataTypes.Instrument);
            entity.Property(e => e.Spread).IsRequired().HasColumnType(SqlDataTypes.Instrument);
            entity.Property(e => e.Filled).IsRequired().HasColumnType(SqlDataTypes.Instrument);
            entity.Property(e => e.ActualSpread).IsRequired().HasColumnType(SqlDataTypes.Instrument);
            entity.Property(e => e.IsInstant).IsRequired();
            entity.Property(e => e.IsCanceled).IsRequired();
            entity.Property(e => e.OpenDate).HasColumnType(SqlDataTypes.Date);
            entity.Property(e => e.UpdateDate).HasColumnType(SqlDataTypes.Date);
            entity.Property(e => e.PositionDemandState).IsRequired();
            entity.Property(e => e.PositionDemandSide).IsRequired();
        }
    }
}

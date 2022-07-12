using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OkxPerpetualArbitrage.Domain.Entities;

namespace OkxPerpetualArbitrage.Persistance.Configurations
{
    class PositionHistoryConfiguration : IEntityTypeConfiguration<PositionHistory>
    {
        public void Configure(EntityTypeBuilder<PositionHistory> entity)
        {
            entity.HasKey(x => x.PositionHistoryId);
            entity.Property(e => e.Symbol).IsRequired().HasColumnType(SqlDataTypes.ShortString);
            entity.Property(e => e.Fee).IsRequired().HasColumnType(SqlDataTypes.Instrument);
            entity.Property(e => e.Funding).IsRequired().HasColumnType(SqlDataTypes.Instrument);
            entity.Property(e => e.PerpTrade).IsRequired().HasColumnType(SqlDataTypes.Instrument);
            entity.Property(e => e.SpotTrade).IsRequired().HasColumnType(SqlDataTypes.Instrument);
            entity.Property(e => e.TotalPNL).IsRequired().HasColumnType(SqlDataTypes.Instrument);
            entity.Property(e => e.OpenDate).HasColumnType(SqlDataTypes.Date);
            entity.Property(e => e.CloseDate).HasColumnType(SqlDataTypes.Date);
        }
    }
}

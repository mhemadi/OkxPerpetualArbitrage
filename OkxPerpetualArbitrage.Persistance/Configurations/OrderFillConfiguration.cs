using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OkxPerpetualArbitrage.Domain.Entities;

namespace OkxPerpetualArbitrage.Persistance.Configurations
{
    public class OrderFillConfiguration : IEntityTypeConfiguration<OrderFill>
    {
        public void Configure(EntityTypeBuilder<OrderFill> entity)
        {
            entity.HasKey(e => e.OrderFillId);
            entity.Property(e => e.ClientOrderId).IsRequired().HasColumnType(SqlDataTypes.MediumString);
            entity.Property(e => e.Size).IsRequired().HasColumnType(SqlDataTypes.Instrument);
            entity.Property(e => e.Lot).IsRequired().HasColumnType(SqlDataTypes.Instrument);
            entity.Property(e => e.Filled).IsRequired().HasColumnType(SqlDataTypes.Instrument);
            entity.Property(e => e.Price).IsRequired().HasColumnType(SqlDataTypes.Instrument);
            entity.Property(e => e.Fee).IsRequired().HasColumnType(SqlDataTypes.Instrument);
            entity.Property(e => e.FeeCurrency).IsRequired().HasColumnType(SqlDataTypes.MediumString);
            entity.Property(e => e.TimeStamp).HasColumnType(SqlDataTypes.Date);
            entity.Property(e => e.PartInPosition).IsRequired();
        }

    }
}

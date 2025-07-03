using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ReservationSystem.Domain.Entities;

namespace ReservationSystem.Infrastructure.Context.Configurations
{
    public class ItemConfiguration : IEntityTypeConfiguration<Item>
    {
        public void Configure(EntityTypeBuilder<Item> builder)
        {
            builder.HasKey(i => i.Id);
            builder.Property(i => i.PricePerHour).HasColumnType("decimal(18,2)");

            builder.HasMany(i => i.Reservations)
                   .WithOne(r => r.Item)
                   .HasForeignKey(r => r.ItemId);
        }
    }
}

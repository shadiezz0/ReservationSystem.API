
namespace ReservationSystem.Infrastructure.Context.Configurations
{
    public class ItemTypeConfiguration : IEntityTypeConfiguration<ItemType>
    {
        public void Configure(EntityTypeBuilder<ItemType> builder)
        {
            builder.HasKey(i => i.Id);

            builder.HasMany(i => i.Items)
                   .WithOne(item => item.ItemType)
                   .HasForeignKey(item => item.ItemTypeId);
        }
    }
}

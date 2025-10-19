
namespace ReservationSystem.Infrastructure.Context.Configurations
{
    public class ItemTypeConfiguration : IEntityTypeConfiguration<ItemType>
    {
        public void Configure(EntityTypeBuilder<ItemType> builder)
        {
            builder.HasKey(i => i.Id);

        
        }
    }
}

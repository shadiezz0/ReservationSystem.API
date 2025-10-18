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

            // Configure relationship with User (CreatedBy)
            builder.HasOne(i => i.CreatedBy)
                   .WithMany(u => u.CreatedItems)
                   .HasForeignKey(i => i.CreatedById)
                   .OnDelete(DeleteBehavior.Restrict); // Prevent deleting user if they have created items

            builder.HasIndex(p => p.AdminId)
                   .IsUnique();
        }
    }
}

namespace ReservationSystem.Infrastructure.Context.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(u => u.Id);

            builder.HasMany(u => u.Reservations)
                   .WithOne(r => r.User)
                   .HasForeignKey(r => r.UserId);


            builder.HasData(new User
            {
                Id = 1,
                Name = "Sh",
                Email = "Sh@example.com", // ✅ REQUIRED field
                PasswordHash = "Sh",
                RoleId = 1
            });
        }
    }
}

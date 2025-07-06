namespace ReservationSystem.Infrastructure.Context.Configurations
{
    public class RoleCofigration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.HasData(new Role
            {
                Id = 1,
                Name = "SuperAdmin"
            });
        }
    }
}

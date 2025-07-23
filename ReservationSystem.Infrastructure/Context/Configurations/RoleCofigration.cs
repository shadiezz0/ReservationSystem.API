using static ReservationSystem.Domain.Constants.Enums;

namespace ReservationSystem.Infrastructure.Context.Configurations
{
    public class RoleCofigration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.HasData(new Role { Id = 1, Name = "SuperAdmin", RoleType = RoleType.SuperAdmin },
                            new Role { Id = 2, Name = "User", RoleType = RoleType.User }
                            );
        }
    }
}

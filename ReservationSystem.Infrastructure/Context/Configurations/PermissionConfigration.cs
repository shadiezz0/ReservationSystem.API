
namespace ReservationSystem.Infrastructure.Context.Configurations
{
    public class PermissionConfigration : IEntityTypeConfiguration<Permission>
    {
        public void Configure(EntityTypeBuilder<Permission> builder)
        {
            builder.HasData(
              new Permission { Id = 1, Name = "CanManageUsers" },
              new Permission { Id = 2, Name = "CanManageRoles" },
              new Permission { Id = 3, Name = "CanManageSettings" },
              new Permission { Id = 4, Name = "CanViewDashboard" }
          );
        }
    }
}

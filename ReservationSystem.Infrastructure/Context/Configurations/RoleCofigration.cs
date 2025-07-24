using static ReservationSystem.Domain.Constants.Enums;

namespace ReservationSystem.Infrastructure.Context.Configurations
{
    public class RoleCofigration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
        }
    }
}

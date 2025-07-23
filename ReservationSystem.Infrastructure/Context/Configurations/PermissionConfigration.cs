namespace ReservationSystem.Infrastructure.Context.Configurations
{
    public class PermissionConfigration : IEntityTypeConfiguration<Permission>
    {
        public void Configure(EntityTypeBuilder<Permission> builder)
        {
            builder.HasData(
                new Permission { Id = 1, Resource = "Reservation", isShow = true, isAdd = true, isEdit = true, isDelete = true },
                new Permission { Id = 2, Resource = "Reservation", isShow = false, isAdd = false, isEdit = false, isDelete = false }
            );
        }
    }
}


namespace ReservationSystem.Infrastructure.Context.Configurations
{
    public class PermissionConfigration : IEntityTypeConfiguration<Permission>
    {
        public void Configure(EntityTypeBuilder<Permission> builder)
        {
            builder.HasData(
              new Permission { Id = 1, isAdd =true,isShow=true , isEdit = true , isDelete=true }
     
          );
        }
    }
}

namespace ReservationSystem.Infrastructure.Context.Configurations
{
    public class RolePermissionConfigration : IEntityTypeConfiguration<RolePermission>
    {
        public void Configure(EntityTypeBuilder<RolePermission> modelBuilder)
        {
            modelBuilder.HasKey(rp => new { rp.RoleId, rp.PermissionId });

            modelBuilder
                .HasOne(rp => rp.Role)
                .WithMany(r => r.RolePermissions)
                .HasForeignKey(rp => rp.RoleId);

            modelBuilder
                .HasOne(rp => rp.Permission)
                .WithMany(p => p.RolePermissions)
                .HasForeignKey(rp => rp.PermissionId);

        }
    }
}


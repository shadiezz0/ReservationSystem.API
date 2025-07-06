namespace ReservationSystem.Domain.Entities
{
    public class Permission
    {
        public int Id { get; set; }
        public string Name { get; set; }

        // Many Permissions → Many Roles
        public ICollection<RolePermission> RolePermissions { get; set; }
    }

}

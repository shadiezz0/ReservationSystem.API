using static ReservationSystem.Domain.Constants.Enums;

namespace ReservationSystem.Domain.Entities
{
    public class Role
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public RoleType RoleType { get; set; }

        // One Role → Many Users
        public ICollection<User> Users { get; set; }

        // Role → many-to-many → Permissions
        public ICollection<RolePermission> RolePermissions { get; set; }
    }


}

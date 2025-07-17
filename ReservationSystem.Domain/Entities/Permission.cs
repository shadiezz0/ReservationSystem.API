namespace ReservationSystem.Domain.Entities
{
    public class Permission
    {
        public int Id { get; set; }
        public string Resource { get; set; }
        public bool isShow { get; set; }
        public bool isAdd { get; set; }
        public bool isEdit { get; set; }
        public bool isDelete { get; set; }

        // Many Permissions → Many Roles
        public ICollection<RolePermission> RolePermissions { get; set; }
    }

}

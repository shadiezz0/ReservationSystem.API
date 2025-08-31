
namespace ReservationSystem.Application.DTOs
{
    public class CreateRoleDto
    {
        public string Name { get; set; }
        public RoleType RoleType { get; set; }
    }
    
    public class UpdateRoleDto : CreateRoleDto
    {
    }
    
    public class GetRoleDto : CreateRoleDto
    {
        public int Id { get; set; }
    }

    public class UpdateRolePermissionsDto
    {
        public List<int> PermissionIds { get; set; }
    }

    public class RoleDetailDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public RoleType RoleType { get; set; }
        public List<PermissionDto> Permissions { get; set; } = new List<PermissionDto>();
        public int UserCount { get; set; }
    }

    public class PermissionDto
    {
        public int Id { get; set; }
        public ResourceType Resource { get; set; }
        public bool IsShow { get; set; }
        public bool IsAdd { get; set; }
        public bool IsEdit { get; set; }
        public bool IsDelete { get; set; }
    }
}
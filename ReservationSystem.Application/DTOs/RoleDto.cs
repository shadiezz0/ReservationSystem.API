
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

    public class RoleDetailDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public RoleType RoleType { get; set; }
        public List<PermissionDto> Permissions { get; set; } = new List<PermissionDto>();
        public int UserCount { get; set; }
    }


}
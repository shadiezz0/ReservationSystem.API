namespace ReservationSystem.Application.IService
{
    public interface IRoleService
    {
        Task<List<ResponseResult>> GetAllRolesAsync();
        Task<ResponseResult> CreateRoleAsync(RoleDto dto);
        Task<List<ResponseResult>> GetPermissionsForRoleAsync(int roleId);
        Task AssignPermissionsToRoleAsync(int roleId, List<int> permissionIds);
    }

}

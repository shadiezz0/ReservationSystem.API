namespace ReservationSystem.Application.IService
{
    public interface IRoleService
    {
        Task<ResponseResult> GetAllRolesAsync();
        Task<ResponseResult> CreateRoleAsync(CreateRoleDto dto);
        Task<ResponseResult> GetPermissionsForRoleAsync(int roleId);
        Task AssignPermissionsToRoleAsync(int roleId, List<int> permissionIds);
    }

}

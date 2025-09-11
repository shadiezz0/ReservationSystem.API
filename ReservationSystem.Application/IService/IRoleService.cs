namespace ReservationSystem.Application.IService
{
    public interface IRoleService
    {
        Task<ResponseResult> GetAllRolesAsync();
        Task<ResponseResult> GetByIdAsync(int id);
        Task<ResponseResult> CreateRoleAsync(CreateRoleDto dto);
        Task<ResponseResult> UpdateRoleAsync(int id, UpdateRoleDto dto);
        Task<ResponseResult> DeleteRoleAsync(int id);
        Task<ResponseResult> GetPermissionsForRoleAsync(int roleId);
        Task<ResponseResult> AssignPermissionsToRoleAsync(int roleId, List<int> permissionIds);
        Task<ResponseResult> GetCurrentUserProfileAsync();

    }

}

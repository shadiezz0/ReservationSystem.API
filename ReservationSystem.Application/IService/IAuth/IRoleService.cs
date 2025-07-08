namespace ReservationSystem.Application.IService.IAuth
{
    public interface IRoleService
    {
        Task<ResponseResult> AssignPermissionsToRole(int roleId, List<int> permissionIds);
        Task<ResponseResult> GetPermissionsByRole(int roleId);
    }
}

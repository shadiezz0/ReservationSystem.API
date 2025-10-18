namespace ReservationSystem.Application.IService
{
    public interface IPermissionCheckerService
    {
        Task<ResponseResult> HasPermissionAsync(ResourceType resource, PermissionAction action);
        Task<ResponseResult> CreatePermissionAsync(CreatePermissionDto dto);
    }

}

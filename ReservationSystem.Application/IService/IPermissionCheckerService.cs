namespace ReservationSystem.Application.IService
{
    public interface IPermissionCheckerService
    {
        Task<ResponseResult> HasPermissionAsync(string resource, string action);
    }

}

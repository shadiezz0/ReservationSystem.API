namespace ReservationSystem.Application.IService
{
    public interface IPermissionService
    {
        Task<ResponseResult> GetAllAsync();
        Task<ResponseResult> GetByIdAsync(int id);
        Task<ResponseResult> CreateAsync(CreatePermissionDto dto);
        Task<ResponseResult> UpdateAsync(UpdatePermissionDto dto);
        Task<ResponseResult> DeleteAsync(int id);

    }

}

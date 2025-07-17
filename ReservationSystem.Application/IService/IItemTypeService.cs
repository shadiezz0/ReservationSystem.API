namespace ReservationSystem.Application.IService
{
    public interface IItemTypeService
    {
        Task<ResponseResult> CreateAsync(CreateItemTypeDto dto);
        Task<ResponseResult> DeleteAsync(int id);
        Task<ResponseResult> UpdateAsync(UpdateItemTypeDto dto);
        Task<ResponseResult> GetAllAsync();
        Task<ResponseResult> GetByIdAsync(int id);

    }
}

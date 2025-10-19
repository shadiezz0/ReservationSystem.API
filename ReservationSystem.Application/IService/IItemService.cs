namespace ReservationSystem.Application.IService
{
    public interface IItemService
    {
        Task<ResponseResult> CreateAsync(CreateItemDto dto);
        Task<ResponseResult> UpdateAsync(UpdateItemDto dto);
        Task<ResponseResult> DeleteAsync(int id);
        Task<ResponseResult> GetAllAsync();
        Task<ResponseResult> GetByIdAsync(int id);
        Task<ResponseResult> FilterByTypeAsync(int itemTypeId);
       // Task<ResponseResult> GetItemByAdminAsync();
    }
}

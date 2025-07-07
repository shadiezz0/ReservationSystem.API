namespace ReservationSystem.Application.IService.IItemType
{
    public interface IItemTypeService
    {
        Task<ResponseResult> CreateAsync(CreateItemTypeDto dto);
        Task<ResponseResult> DeleteAsync(int id);
        Task<ResponseResult> GetAllAsync();
    }
}

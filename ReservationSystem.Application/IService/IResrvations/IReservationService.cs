namespace ReservationSystem.Application.IService.IResrvations
{
    public interface IReservationService
    {
        Task<ResponseResult> CreateAsync(CreateReservationDto dto);
        Task<ResponseResult> UpdateAsync(UpdateReservationDto dto);
        Task<ResponseResult> DeleteAsync(int id);
        Task<ResponseResult> GetAllAsync();
        Task<ResponseResult> GetByIdAsync(int id);
    }
}

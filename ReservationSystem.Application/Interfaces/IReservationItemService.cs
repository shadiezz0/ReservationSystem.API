using ReservationSystem.Application.DTOs;
using ReservationSystem.Domain.Entities;

namespace ReservationSystem.Application.Interfaces
{
    public interface IReservationItemService
    {
        Task<List<ReservationItem>> GetAllAsync();
        Task<ReservationItem?> GetByIdAsync(int id);
        Task AddAsync(ReservationItemDto dto);
        Task UpdateAsync(int id, ReservationItemDto dto);
        Task DeleteAsync(int id);
    }
}

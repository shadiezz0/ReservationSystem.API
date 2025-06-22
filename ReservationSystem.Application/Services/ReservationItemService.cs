using AutoMapper;
using ReservationSystem.Application.DTOs;
using ReservationSystem.Application.Interfaces;
using ReservationSystem.Domain.Entities;

namespace ReservationSystem.Application.Services
{
    public class ReservationItemService : IReservationItemService
    {
        private readonly IRepository<ReservationItem> ReservationItem_repository;
        private readonly IMapper _mapper;

        public ReservationItemService(IMapper mapper, IRepository<ReservationItem> reservationItem_repository)
        {
            _mapper = mapper;
            ReservationItem_repository = reservationItem_repository;
        }

        public async Task AddAsync(ReservationItemDto dto)
        {
            var reservationItem = _mapper.Map<ReservationItem>(dto);
            await ReservationItem_repository.AddAsync(reservationItem);
            await ReservationItem_repository.SaveAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var item = await ReservationItem_repository.GetByIdAsync(id);
            if (item == null) return;
            ReservationItem_repository.Delete(item);
            await ReservationItem_repository.SaveAsync();
        }

        public async Task<List<ReservationItem>> GetAllAsync() => await ReservationItem_repository.GetAllAsync();

        public async Task<ReservationItem?> GetByIdAsync(int id) => await ReservationItem_repository.GetByIdAsync(id);

        public async Task UpdateAsync(int id, ReservationItemDto dto)
        {
            var item = await ReservationItem_repository.GetByIdAsync(id);
            if (item == null) return;
            _mapper.Map(dto, item);
            ReservationItem_repository.Update(item);
            await ReservationItem_repository.SaveAsync();

        }
    }
}

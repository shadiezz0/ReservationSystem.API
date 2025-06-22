using AutoMapper;
using ReservationSystem.Application.DTOs;
using ReservationSystem.Domain.Entities;

namespace ReservationSystem.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ReservationItemDto, ReservationItem>().ReverseMap();
            CreateMap<ReservationDto, Reservation>().ReverseMap();
        }
    }
}

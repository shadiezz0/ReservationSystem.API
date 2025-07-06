using AutoMapper;

namespace ReservationSystem.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Reservation, ReservationDto>();         
  
            CreateMap<Item, ItemDto>();
            CreateMap<ItemType, ItemTypeDto>();

        }
    }
}

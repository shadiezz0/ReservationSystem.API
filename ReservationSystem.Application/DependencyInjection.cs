using Microsoft.Extensions.DependencyInjection;
using ReservationSystem.Application.Interfaces;
using ReservationSystem.Application.Mappings;
using ReservationSystem.Application.Services;

namespace ReservationSystem.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IReservationItemService, ReservationItemService>();
            services.AddAutoMapper(typeof(MappingProfile).Assembly);
            return services;

        }
    }
}

using Microsoft.Extensions.DependencyInjection;
using ReservationSystem.Application.IService.IResrvations;
using ReservationSystem.Application.Mappings;
using ReservationSystem.Application.Service.Resrvations;

namespace ReservationSystem.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(MappingProfile).Assembly);
            services.AddScoped<IReservationService, ReservationService>();
            return services;
        }
    }
}

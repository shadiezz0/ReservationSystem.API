using Microsoft.Extensions.DependencyInjection;
using ReservationSystem.Application.IService;
using ReservationSystem.Application.Mappings;
using ReservationSystem.Application.Service;

namespace ReservationSystem.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(MappingProfile).Assembly);
            services.AddScoped<IReservationService, ReservationService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ITokenService, TokenService>();
            return services;
        }
    }
}

using Microsoft.Extensions.DependencyInjection;
using ReservationSystem.Application.Mappings;

namespace ReservationSystem.Application
{
      public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(MappingProfile).Assembly);
            return services;

        }
    }
}

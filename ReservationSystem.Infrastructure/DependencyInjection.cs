using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ReservationSystem.Domain.Interfaces;
using ReservationSystem.Infrastructure.Context;
using ReservationSystem.Infrastructure.Repositories;

namespace ReservationSystem.Infrastructure
{
      public static class DependencyInjection
      {
            public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
            {
                  services.AddDbContext<AppDbContext>(options =>
                     options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

                  services.AddScoped<IUnitOfWork, UnitOfWork>();

                  return services;
            }


      }
}

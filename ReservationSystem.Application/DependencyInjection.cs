using Microsoft.Extensions.DependencyInjection;
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
            services.AddScoped<IPermissionCheckerService, PermissionCheckerService>();
            services.AddScoped<IItemTypeService, ItemTypeService>();
            services.AddScoped<IItemService, ItemService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddHttpContextAccessor();

            return services;
        }
    }
}

using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace ReservationSystem.Application.Service
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public int? GetCurrentUserId()
        {
            var userIdString = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
                return null;
            
            return userId;
        }

        public string? GetCurrentUserRole()
        {
            return _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Role)?.Value;
        }

        public bool IsCurrentUserSuperAdmin()
        {
            return GetCurrentUserRole() == "SuperAdmin";
        }

        public bool IsCurrentUserAdmin()
        {
            var role = GetCurrentUserRole();
            return role == "Admin" || role == "SuperAdmin";
        }
    }
}
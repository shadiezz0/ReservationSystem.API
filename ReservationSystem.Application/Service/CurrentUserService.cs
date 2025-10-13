using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace ReservationSystem.Application.Service
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IGenericRepository<User> _userRepo;
        public CurrentUserService(IHttpContextAccessor httpContextAccessor,IGenericRepository<User> userRepo)
        {
            _httpContextAccessor = httpContextAccessor;
            _userRepo = userRepo;
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

        public async Task<bool> IsAdmin(int? userId)
        {
            var user = await _userRepo.FindOneAsync(a => a.Id == userId);
            var role = user.RoleId;
            return role == (int)RoleType.Admin ;
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
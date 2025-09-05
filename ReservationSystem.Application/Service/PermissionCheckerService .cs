using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace ReservationSystem.Application.Service
{
    public class PermissionCheckerService : IPermissionCheckerService
    {
        private readonly IGenericRepository<User> _userRepo;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PermissionCheckerService(IUnitOfWork uow, IHttpContextAccessor httpContextAccessor)
        {
            _userRepo = uow.Repository<User>();
            _httpContextAccessor = httpContextAccessor;
        }

        public ResponseResult ReturnUnAuthorized => new ResponseResult()
        {
            Result = Result.Unauthorized,
            Alart = new Alart
            {
                AlartType = AlartType.Unauthorized,
                type = AlartShow.popup,
                MessageEn = "You do not have permission to perform this action.",
                MessageAr = "ليس لديك إذن لأداء هذا الإجراء."
            }
        };

        public async Task<ResponseResult> HasPermissionAsync(ResourceType resource, PermissionAction action)
        {
            var userIdString = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return ReturnUnAuthorized;
            }

            var user = await _userRepo
                        .FindOneAsync(
                            predicate: u => u.Id == userId,
                            include: query => query
                                .Include(u => u.Role)
                                    .ThenInclude(r => r.RolePermissions)
                                        .ThenInclude(rp => rp.Permission)
                        );


            if (user == null)
                return ReturnUnAuthorized;

            if (user.Role.RoleType == RoleType.SuperAdmin)
                return null;

            var permission = user.Role.RolePermissions
        .Select(rp => rp.Permission)
        .FirstOrDefault(p => p.Resource == resource);

            if (permission == null)
                return ReturnUnAuthorized;

            var isAuthorized = action switch
            {
                PermissionAction.Show => permission.isShow,
                PermissionAction.Add => permission.isAdd,
                PermissionAction.Edit => permission.isEdit,
                PermissionAction.Delete => permission.isDelete,
                _ => false
            };

            return isAuthorized ? null : ReturnUnAuthorized;

        }
    }

}

using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ReservationSystem.Infrastructure.Context;

namespace ReservationSystem.Application.Service
{
    public class PermissionCheckerService : IPermissionCheckerService
    {
        private readonly IGenericRepository<User> _userRepo;
        private readonly IGenericRepository<Permission> _permissionRepo;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AppDbContext _context;
        public PermissionCheckerService(IUnitOfWork uow, IHttpContextAccessor httpContextAccessor, AppDbContext context, IGenericRepository<Permission> permissionRepo)
        {
            _userRepo = uow.Repository<User>();
            _httpContextAccessor = httpContextAccessor;
            _context = context;
            _permissionRepo = permissionRepo;
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


        public async Task<ResponseResult> CreatePermissionAsync(CreatePermissionDto dto)
        {
            var permission = new Permission
            {
                Resource = dto.Resource,
                isShow = dto.IsShow,
                isAdd = dto.IsAdd,
                isEdit = dto.IsEdit,
                isDelete = dto.IsDelete
            };

            // Check for duplicates
            var exists = await _context.Permissions.Where(a =>
                a.Resource == permission.Resource &&
                a.isShow == permission.isShow &&
                a.isEdit == permission.isEdit &&
                a.isAdd == permission.isAdd &&
                a.isDelete == permission.isDelete)?.FirstOrDefaultAsync();
            var rolePermission = new RolePermission();

            if (exists == null)
            {

                // Save Permission
                _context.Permissions.Add(permission);
                await _context.SaveChangesAsync();

                // Assign to Role

                rolePermission.RoleId = dto.RoleId;
                rolePermission.PermissionId = permission.Id;
                

            }
            else
            {
                rolePermission.RoleId = dto.RoleId;
                rolePermission.PermissionId = exists.Id;
            }

            var existsRolePermission = await _context.RolePermissions.AnyAsync(a =>
            a.PermissionId == rolePermission.PermissionId 
            && a.RoleId == rolePermission.RoleId 
              );
            if (existsRolePermission)
            {
                return new ResponseResult()
                {
                    Alart = new Alart
                    {
                        AlartType = AlartType.warning,
                        type = AlartShow.popup,
                        MessageEn = "Permission already exists.",

                    }
                };
            }
            _context.RolePermissions.Add(rolePermission);
            await _context.SaveChangesAsync();
            return new ResponseResult()
            {
                Data = rolePermission,
                Alart = new Alart
                {
                    AlartType = AlartType.success,
                    type = AlartShow.popup,
                    MessageEn = "Permission created and assigned to role successfully.",

                }
            };

        }
    }

}

using ReservationSystem.Application.IService.IAuth;

namespace ReservationSystem.Application.Service.Auth
{
    public class RoleService : IRoleService
    {
        private readonly IGenericRepository<RolePermission> _rolePermRepo;
        private readonly IUnitOfWork _uow;

        public RoleService(IUnitOfWork uow)
        {
            _uow = uow;
            _rolePermRepo = _uow.Repository<RolePermission>();
        }
        public async Task<ResponseResult> AssignPermissionsToRole(int roleId, List<int> permissionIds)
        {
            // Validate input
            if (permissionIds == null || !permissionIds.Any())
            {
                return new ResponseResult
                {
                    Result = Result.Failed,
                    Alart = new Alart
                    {
                        AlartType = AlartType.error,
                        type = AlartShow.note,
                        MessageAr = "قائمة الأذونات فارغة.",
                        MessageEn = "Permission list is empty.",
                    }
                };
            }

            // Fetch existing permissions for the role
            var existingPermissions = await _rolePermRepo.FindAllAsync(rp => rp.RoleId == roleId);

            // Remove existing permissions
            if (existingPermissions.Any())
            {
                foreach (var permission in existingPermissions)
                {
                    _rolePermRepo.Delete(permission);
                }
            }

            // Assign new permissions
            var newPermissions = permissionIds.Select(pid => new RolePermission { RoleId = roleId, PermissionId = pid });
            foreach (var permission in newPermissions)
            {
                await _rolePermRepo.AddAsync(permission);
            }

            // Save changes
            var saveResult = await _uow.SaveAsync();
            if (!saveResult)
            {
                return new ResponseResult
                {
                    Result = Result.Failed,
                    Alart = new Alart
                    {
                        AlartType = AlartType.error,
                        type = AlartShow.note,
                        MessageAr = "فشل حفظ التغييرات.",
                        MessageEn = "Failed to save changes.",
                    }
                };
            }

            // Return success response
            return new ResponseResult
            {
                Data = new { role_id = roleId, permissions_assigned = permissionIds },
                Result = Result.Success,
                Alart = new Alart
                {
                    AlartType = AlartType.success,
                    type = AlartShow.note,
                    MessageAr = "تم تعيين الأذونات بنجاح.",
                    MessageEn = "Permissions assigned successfully.",
                }
            };
        }

        public async Task<ResponseResult> GetPermissionsByRole(int roleId)
        {
            // Fetch permissions for the role
            var permissions = await _rolePermRepo.FindAllAsync(rp => rp.RoleId == roleId);
            // Check if any permissions found
            if (!permissions.Any())
            {
                return new ResponseResult
                {
                    Result = Result.NoDataFound,
                    Alart = new Alart
                    {
                        AlartType = AlartType.warning,
                        type = AlartShow.note,
                        MessageAr = "لا توجد أذونات لهذا الدور.",
                        MessageEn = "No permissions found for this role.",
                    }
                };
            }
            // Return the permissions
            return new ResponseResult
            {
                Data = permissions.Select(p => p.PermissionId).ToList(),
                Result = Result.Success,
                TotalCount = permissions.Count(),
                Alart = new Alart
                {
                    AlartType = AlartType.success,
                    type = AlartShow.note,
                    MessageAr = "تم استرجاع الأذونات بنجاح.",
                    MessageEn = "Permissions retrieved successfully.",
                }
            };

        }
    }
}

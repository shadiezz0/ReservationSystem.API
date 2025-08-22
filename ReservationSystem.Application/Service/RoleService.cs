using ReservationSystem.Domain.Entities;

namespace ReservationSystem.Application.Service
{
    public class RoleService : IRoleService
    {
        private readonly IUnitOfWork _uow;
        private readonly IGenericRepository<Role> _roleRepo;
        private readonly IGenericRepository<Permission> _permissionRepo;
        private readonly IGenericRepository<RolePermission> _rolePermissionRepo;

        public RoleService(IUnitOfWork uow)
        {
            _uow = uow;
            _roleRepo = uow.Repository<Role>();
            _permissionRepo = uow.Repository<Permission>();
            _rolePermissionRepo = uow.Repository<RolePermission>();
        }

        public async Task<ResponseResult> GetAllRolesAsync()
        {
            var roles =  await _roleRepo.GetAllAsync(asNoTracking:true);
            var roleDtos = roles.Select(r => new GetRoleDto
            {
                Id = r.Id,
                Name = r.Name,
                RoleType = r.RoleType
            }).ToList();

            return new ResponseResult
            {
                Data = roleDtos,
                Result = Result.Success,
                Alart = new Alart
                {
                    AlartType = AlartType.success,
                    type = AlartShow.note,
                    MessageEn = "Roles retrieved successfully",
                    MessageAr = "تم استرجاع الأدوار بنجاح"
                }
            };

        }

        public async Task<ResponseResult> CreateRoleAsync(CreateRoleDto dto)
        {
            var existing = await _roleRepo.FindOneAsync(r => r.Name == dto.Name);
            if (existing != null) throw new Exception("Role already exists");

            var role = new Role
            {
                Name = dto.Name,
                RoleType = dto.RoleType
            };

            await _roleRepo.AddAsync(role);
            await _uow.SaveAsync();

            return new ResponseResult
            {
                Data = role.Id,
                Result = Result.Success,
                Alart = new Alart
                {
                    AlartType = AlartType.success,
                    type = AlartShow.note,
                    MessageEn = "Role created successfully",
                    MessageAr = "تم إنشاء الدور بنجاح"
                }
            };
        }

        public async Task<ResponseResult> GetPermissionsForRoleAsync(int roleId)
        {
            var rolePermissions = await _rolePermissionRepo.FindAllAsync(rp => rp.RoleId == roleId);
            return new ResponseResult
            {
                Data = rolePermissions,
                Result = Result.Success,
            };
        }   

        public async Task AssignPermissionsToRoleAsync(int roleId, List<int> permissionIds)
        {
            var existing = await _rolePermissionRepo.FindAllAsync(rp => rp.RoleId == roleId);
            var assigned = existing.Select(e => e.PermissionId).ToHashSet();

            // Add missing
            foreach (var pid in permissionIds)
            {
                if (!assigned.Contains(pid))
                {
                    await _rolePermissionRepo.AddAsync(new RolePermission
                    {
                        RoleId = roleId,
                        PermissionId = pid
                    });
                }
            }

            // Remove unselected
            foreach (var rp in existing)
            {
                if (!permissionIds.Contains(rp.PermissionId))
                {
                    _rolePermissionRepo.Delete(rp);
                }
            }

            await _uow.SaveAsync();
        }
    }

}

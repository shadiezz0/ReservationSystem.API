using Microsoft.EntityFrameworkCore;

namespace ReservationSystem.Application.Service
{
    public class RoleService : IRoleService
    {
        private readonly IUnitOfWork _uow;
        private readonly IGenericRepository<Role> _roleRepo;
        private readonly IGenericRepository<RolePermission> _rolePermissionRepo;

        public RoleService(IUnitOfWork uow)
        {
            _uow = uow;
            _roleRepo = uow.Repository<Role>();
            _rolePermissionRepo = uow.Repository<RolePermission>();
        }

        public async Task<ResponseResult> GetAllRolesAsync()
        {
            var roles = await _roleRepo.GetAllAsync(asNoTracking: true);
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

        public async Task<ResponseResult> GetByIdAsync(int id)
        {
            var role = await _roleRepo.FindOneAsync(
                r => r.Id == id,
                include: query => query
                    .Include(r => r.RolePermissions)
                        .ThenInclude(rp => rp.Permission)
                    .Include(r => r.Users)
            );

            if (role == null)
            {
                return new ResponseResult
                {
                    Result = Result.NotExsit,
                    Alart = new Alart
                    {
                        AlartType = AlartType.error,
                        type = AlartShow.note,
                        MessageEn = "Role not found",
                        MessageAr = "الدور غير موجود"
                    }
                };
            }

            var roleDto = new RoleDetailDto
            {
                Id = role.Id,
                Name = role.Name,
                RoleType = role.RoleType,
                UserCount = role.Users?.Count ?? 0,
                Permissions = role.RolePermissions?.Select(rp => new PermissionDto
                {
                    Id = rp.Permission.Id,
                    Resource = rp.Permission.Resource,
                    IsShow = rp.Permission.isShow,
                    IsAdd = rp.Permission.isAdd,
                    IsEdit = rp.Permission.isEdit,
                    IsDelete = rp.Permission.isDelete
                }).ToList() ?? new List<PermissionDto>()
            };

            return new ResponseResult
            {
                Data = roleDto,
                Result = Result.Success
            };
        }

        public async Task<ResponseResult> CreateRoleAsync(CreateRoleDto dto)
        {
            var existing = await _roleRepo.FindOneAsync(r => r.Name == dto.Name);
            if (existing != null)
            {
                return new ResponseResult
                {
                    Result = Result.Exist,
                    Alart = new Alart
                    {
                        AlartType = AlartType.error,
                        type = AlartShow.note,
                        MessageEn = "Role already exists",
                        MessageAr = "الدور موجود بالفعل"
                    }
                };
            }

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

        public async Task<ResponseResult> UpdateRoleAsync(int id, UpdateRoleDto dto)
        {
            var role = await _roleRepo.FindOneAsync(r => r.Id == id);
            if (role == null)
            {
                return new ResponseResult
                {
                    Result = Result.NotExsit,
                    Alart = new Alart
                    {
                        AlartType = AlartType.error,
                        type = AlartShow.note,
                        MessageEn = "Role not found",
                        MessageAr = "الدور غير موجود"
                    }
                };
            }

            var existing = await _roleRepo.FindOneAsync(r => r.Name == dto.Name && r.Id != id);
            if (existing != null)
            {
                return new ResponseResult
                {
                    Result = Result.Exist,
                    Alart = new Alart
                    {
                        AlartType = AlartType.error,
                        type = AlartShow.note,
                        MessageEn = "Role name already exists",
                        MessageAr = "اسم الدور موجود بالفعل"
                    }
                };
            }

            role.Name = dto.Name;
            role.RoleType = dto.RoleType;

            _roleRepo.Update(role);
            await _uow.SaveAsync();

            return new ResponseResult
            {
                Data = role.Id,
                Result = Result.Success,
                Alart = new Alart
                {
                    AlartType = AlartType.success,
                    type = AlartShow.note,
                    MessageEn = "Role updated successfully",
                    MessageAr = "تم تحديث الدور بنجاح"
                }
            };
        }

        public async Task<ResponseResult> DeleteRoleAsync(int id)
        {
            var role = await _roleRepo.FindOneAsync(r => r.Id == id, include: query => query.Include(r => r.Users));
            if (role == null)
            {
                return new ResponseResult
                {
                    Result = Result.NotExsit,
                    Alart = new Alart
                    {
                        AlartType = AlartType.error,
                        type = AlartShow.note,
                        MessageEn = "Role not found",
                        MessageAr = "الدور غير موجود"
                    }
                };
            }

            if (role.Users != null && role.Users.Any())
            {
                return new ResponseResult
                {
                    Result = Result.Failed,
                    Alart = new Alart
                    {
                        AlartType = AlartType.error,
                        type = AlartShow.note,
                        MessageEn = "Cannot delete role with assigned users",
                        MessageAr = "لا يمكن حذف دور له مستخدمون مُعيَّنون"
                    }
                };
            }

            var rolePermissions = await _rolePermissionRepo.FindAllAsync(rp => rp.RoleId == id);
            foreach (var rp in rolePermissions)
            {
                _rolePermissionRepo.Delete(rp);
            }

            _roleRepo.Delete(role);
            await _uow.SaveAsync();

            return new ResponseResult
            {
                Result = Result.Success,
                Alart = new Alart
                {
                    AlartType = AlartType.success,
                    type = AlartShow.note,
                    MessageEn = "Role deleted successfully",
                    MessageAr = "تم حذف الدور بنجاح"
                }
            };
        }

        public async Task<ResponseResult> GetPermissionsForRoleAsync(int roleId)
        {
            var role = await _roleRepo.FindOneAsync(r => r.Id == roleId);
            if (role == null)
            {
                return new ResponseResult
                {
                    Result = Result.NotExsit,
                    Alart = new Alart
                    {
                        AlartType = AlartType.error,
                        type = AlartShow.note,
                        MessageEn = "Role not found",
                        MessageAr = "الدور غير موجود"
                    }
                };
            }

            var rolePermissions = await _rolePermissionRepo.FindAllAsync(
                rp => rp.RoleId == roleId,
                include: query => query.Include(rp => rp.Permission)
            );

            var permissions = rolePermissions.Select(rp => new PermissionDto
            {
                Id = rp.Permission.Id,
                Resource = rp.Permission.Resource,
                IsShow = rp.Permission.isShow,
                IsAdd = rp.Permission.isAdd,
                IsEdit = rp.Permission.isEdit,
                IsDelete = rp.Permission.isDelete
            }).ToList();

            return new ResponseResult
            {
                Data = permissions,
                Result = Result.Success,
                Alart = new Alart
                {
                    AlartType = AlartType.success,
                    type = AlartShow.note,
                    MessageEn = "Role permissions retrieved successfully",
                    MessageAr = "تم استرجاع صلاحيات الدور بنجاح"
                }
            };
        }

        public async Task<ResponseResult> AssignPermissionsToRoleAsync(int roleId, List<int> permissionIds)
        {
            var role = await _roleRepo.FindOneAsync(r => r.Id == roleId);
            if (role == null)
            {
                return new ResponseResult
                {
                    Result = Result.NotExsit,
                    Alart = new Alart
                    {
                        AlartType = AlartType.error,
                        type = AlartShow.note,
                        MessageEn = "Role not found",
                        MessageAr = "الدور غير موجود"
                    }
                };
            }

            var existing = await _rolePermissionRepo.FindAllAsync(rp => rp.RoleId == roleId);
            var assigned = existing.Select(e => e.PermissionId).ToHashSet();

            // Add missing permissions
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

            // Remove unselected permissions
            foreach (var rp in existing)
            {
                if (!permissionIds.Contains(rp.PermissionId))
                {
                    _rolePermissionRepo.Delete(rp);
                }
            }

            await _uow.SaveAsync();

            return new ResponseResult
            {
                Result = Result.Success,
                Alart = new Alart
                {
                    AlartType = AlartType.success,
                    type = AlartShow.note,
                    MessageEn = "Role permissions updated successfully",
                    MessageAr = "تم تحديث صلاحيات الدور بنجاح"
                }
            };
        }
    }
}

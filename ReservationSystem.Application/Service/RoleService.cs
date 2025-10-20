using Microsoft.EntityFrameworkCore;
using ReservationSystem.Domain.Entities;

namespace ReservationSystem.Application.Service
{
    public class RoleService : IRoleService
    {
        private readonly IUnitOfWork _uow;
        private readonly IAuthService _IAuthService;
        private readonly IGenericRepository<Role> _roleRepo;
        private readonly IGenericRepository<User> _userRepo;
        private readonly IGenericRepository<RolePermission> _rolePermissionRepo;

        public RoleService(IUnitOfWork uow, IAuthService iAuthService)
        {
            _uow = uow;
            _roleRepo = uow.Repository<Role>();
            _rolePermissionRepo = uow.Repository<RolePermission>();
            _userRepo = uow.Repository<User>();
            _IAuthService = iAuthService;
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


        public async Task<ResponseResult> GetCurrentUserProfileAsync()
        {
            var currentUserId = await ResponseHelper.GetCurrentUserId();

            var user = await _userRepo.FindOneAsync(
                u => u.Id == currentUserId,
                include: query => query
                    .Include(u => u.Role)
                    .Include(u => u.Reservations)
                        .ThenInclude(r => r.Item),
                asNoTracking: true
            );

            if (user == null)
                return ResponseHelper.Warning("المستخدم غير موجود", "User not found");

            var userProfile = new UserProfileDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                RoleId = user.RoleId,
                RoleName = user.Role.Name,
                RoleType = user.Role.RoleType,
                ReservationCount = user.Reservations?.Count ?? 0,
            };

            return ResponseHelper.Success("تم استرجاع ملف المستخدم بنجاح", "User profile retrieved successfully", userProfile);
        }
        public async Task<ResponseResult> UpdateUserRoleAsync(int userId, int newRoleId)
        {
            // 1️⃣ Check user exists
            var user = await _userRepo.FindOneAsync(a => a.Id == userId);
            if (user == null)
                return ResponseHelper.Failed("المستخدم غير موجود", "User not found");

            // 2️⃣ (Optional) Validate newRoleId exists in Roles table
            var role = await _roleRepo.FindOneAsync(r => r.Id == newRoleId);
            if (role == null)
                return ResponseHelper.Failed("الدور غير موجود", "Role not found");

            // 3️⃣ Update role
            user.RoleId = newRoleId;
            _userRepo.Update(user);
            await _uow.SaveAsync();

            return ResponseHelper.Success("تم تحديث دور المستخدم بنجاح", "User role updated successfully", new
            {
                UserId = user.Id,
                UserName = user.Name,
                NewRoleId = user.RoleId,
                NewRoleName = role.Name
            });
        }

        public async Task<ResponseResult> GetAllUsersAsync()
        {
            var users = await _userRepo.GetAllAsync(include: query => query
                    .Include(u => u.Role).ThenInclude(r => r.RolePermissions).ThenInclude(p=>p.Permission), asNoTracking: true);
            var userDtos = users.Select(u => new UserWithPermissionsDto
            {
                UserId = u.Id,
                UserName = u.Name,
                RoleName = u.Role?.Name,
                Permissions = u.Role?.RolePermissions
                .Select(rp => new ListPermissionDto
                {
                    Resource = rp.Permission.Resource.ToString(),
                    IsShow = rp.Permission.isShow,
                    IsEdit = rp.Permission.isEdit,
                    IsAdd = rp.Permission.isAdd,
                    IsDelete = rp.Permission.isDelete
                })
        .Distinct()
        .ToList()
            }).ToList();

            return new ResponseResult
            {
                Data = userDtos,
                Result = Result.Success,
                Alart = new Alart
                {
                    AlartType = AlartType.success,
                    type = AlartShow.note,
                    MessageEn = "Users retrieved successfully",
                    MessageAr = "تم استرجاع الأعضاء بنجاح"
                }
            };
        }

     
    }
}


using BCrypt.Net;
using ReservationSystem.Domain.Interfaces;
using static ReservationSystem.Domain.Constants.Enums;

namespace ReservationSystem.Infrastructure.Seeding
{
    public class DataSeeder : IDataSeeder
    {
        private readonly IUnitOfWork _uow;
        private readonly IGenericRepository<Role> _roleRepo;
        private readonly IGenericRepository<Permission> _permissionRepo;
        private readonly IGenericRepository<RolePermission> _rolePermissionRepo;
        private readonly IGenericRepository<User> _userRepo;

        public DataSeeder(IUnitOfWork uow)
        {
            _uow = uow;
            _roleRepo = uow.Repository<Role>();
            _permissionRepo = uow.Repository<Permission>();
            _rolePermissionRepo = uow.Repository<RolePermission>();
            _userRepo = uow.Repository<User>();
        }

        public async Task SeedAsync()
        {
            // Seed Roles if exsists and if not create them
            var superAdmin = await EnsureRoleExists("SuperAdmin", RoleType.SuperAdmin);
            var admin = await EnsureRoleExists("Admin", RoleType.Admin);
            var user = await EnsureRoleExists("User", RoleType.User);

            // Seed Permissions if they do not exist
            var permissions = await SeedPermissions();

            // Assign All Permissions to SuperAdmin
            await AssignPermissionsToRole(superAdmin.Id, permissions);

            // Assign Limited Permissions to Admin (e.g., Items and Reservations only)
            var adminPermissions = permissions.Where(p =>
                   p.Resource == ResourceType.Items ||
                   p.Resource == ResourceType.Reservations).ToList();

            await AssignPermissionsToRole(admin.Id, adminPermissions);

            // Seed SuperAdmin User and If not, creates one
            var existingUser = await _userRepo.FindOneAsync(u => u.Email == "sh@sys.com");
            if (existingUser == null)
            {
                var superUser = new User
                {
                    Name = "sh",
                    Email = "sh@sys.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("sh123"),
                    RoleId = superAdmin.Id
                };
                await _userRepo.AddAsync(superUser);
            }
            await _uow.SaveAsync();
        }

        private async Task<Role> EnsureRoleExists(string name, RoleType type)
        {
            var role = await _roleRepo.FindOneAsync(r => r.Name == name);
            if (role == null)
            {
                role = new Role { Name = name, RoleType = type };
                await _roleRepo.AddAsync(role);
                await _uow.SaveAsync();
            }
            return role;
        }

        private async Task<List<Permission>> SeedPermissions()
        {
            var list = new List<Permission>();

            foreach (var resource in Enum.GetValues<ResourceType>())
            {
                var perm = await _permissionRepo.FindOneAsync(p => p.Resource == resource);
                if (perm == null)
                {
                    perm = new Permission
                    {
                        Resource = resource,
                        isAdd = true,
                        isEdit = true,
                        isDelete = true,
                        isShow = true
                    };
                    await _permissionRepo.AddAsync(perm);
                    list.Add(perm);
                }
                else
                {
                    list.Add(perm);
                }
            }

            await _uow.SaveAsync();
            return list;
        }

        private async Task AssignPermissionsToRole(int roleId, List<Permission> permissions)
        {
            var existing = await _rolePermissionRepo.FindAllAsync(rp => rp.RoleId == roleId);
            var assignedPermissionIds = existing.Select(rp => rp.PermissionId).ToHashSet();

            foreach (var perm in permissions)
            {
                if (!assignedPermissionIds.Contains(perm.Id))
                {
                    await _rolePermissionRepo.AddAsync(new RolePermission
                    {
                        RoleId = roleId,
                        PermissionId = perm.Id
                    });
                }
            }
        }


    }
}

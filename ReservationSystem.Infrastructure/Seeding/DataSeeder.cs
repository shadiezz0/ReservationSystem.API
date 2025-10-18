using BCrypt.Net;
using ReservationSystem.Domain.Entities;
using ReservationSystem.Domain.Interfaces;
using System.Security.AccessControl;
using static ReservationSystem.Domain.Constants.Enums;
using ResourceType = ReservationSystem.Domain.Constants.Enums.ResourceType;


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
            var superAdmin = await EnsureRoleExists("SuperAdmin",RoleType.SuperAdmin);
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

            // User → Show-only on Items + ItemTypes + Reservations
            var userPermissions = permissions
                .Where(p =>
                    (p.Resource == ResourceType.Items ||
                     p.Resource == ResourceType.ItemTypes ||
                     p.Resource == ResourceType.Reservations) &&
                    p.isShow && !p.isAdd && !p.isEdit && !p.isDelete)
                .ToList();
            await AssignPermissionsToRole(user.Id, userPermissions);

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
                await _uow.SaveAsync();
                existingUser = superUser;
            }

            // Handle any existing items that don't have a CreatedById assigned
            await AssignCreatedByToExistingItems(existingUser.Id);

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

        //private async Task<List<Permission>> SeedPermissions()
        //{
        //    var list = new List<Permission>();

        //    foreach (var resource in Enum.GetValues<ResourceType>())
        //    {
        //        // --- Full Access Permission ---
        //        var fullPerm = await _permissionRepo.FindOneAsync(p =>
        //            p.Resource == resource &&
        //            p.isShow == true &&
        //            p.isAdd == true &&
        //            p.isEdit == true &&
        //            p.isDelete == true);

        //        if (fullPerm == null)
        //        {
        //            fullPerm = new Permission
        //            {
        //                Resource = resource,
        //                isShow = true,
        //                isAdd = true,
        //                isEdit = true,
        //                isDelete = true
        //            };
        //            await _permissionRepo.AddAsync(fullPerm);
        //        }
        //        list.Add(fullPerm);

        //        // --- Show Only Permission ---
        //        var showOnlyPerm = await _permissionRepo.FindOneAsync(p =>
        //            p.Resource == resource &&
        //            p.isShow == true &&
        //            p.isAdd == false &&
        //            p.isEdit == false &&
        //            p.isDelete == false);

        //        if (showOnlyPerm == null)
        //        {
        //            showOnlyPerm = new Permission
        //            {
        //                Resource = resource,
        //                isShow = true,
        //                isAdd = false,
        //                isEdit = false,
        //                isDelete = false
        //            };
        //            await _permissionRepo.AddAsync(showOnlyPerm);
        //        }
        //        list.Add(showOnlyPerm);
        //    }

        //    await _uow.SaveAsync();
        //    return list;
        //}


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

        private async Task AssignCreatedByToExistingItems(int superAdminUserId)
        {
            var itemRepo = _uow.Repository<Item>();
            // Find items where CreatedById is null or 0 (default value)
            var itemsWithoutCreator = await itemRepo.FindAllAsync(i => i.CreatedById == null || i.CreatedById == 0);
            
            foreach (var item in itemsWithoutCreator)
            {
                item.CreatedById = superAdminUserId;
                itemRepo.Update(item);
            }
            
            // After assigning creators to all items, save changes
            if (itemsWithoutCreator.Any())
            {
                await _uow.SaveAsync();
                
                // Now create and run a final migration to make the column non-nullable
                // This would typically be done through a separate migration, but for now we'll handle it in code
                await EnsureCreatedByIdIsNotNullable();
            }
        }

        private async Task EnsureCreatedByIdIsNotNullable()
        {
            // This method ensures that after all items have valid CreatedById values,
            // we can safely make the database column non-nullable if needed
            // Note: This is a conceptual method - the actual schema change should be done via migration
        }
    }
}

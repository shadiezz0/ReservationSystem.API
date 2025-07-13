using ReservationSystem.Application.IService.IAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationSystem.Application.Service.Auth
{
    public class PermissionService : IPermissionService
    {
        private readonly IUnitOfWork _uow;
        private readonly IGenericRepository<User> _user;

        public PermissionService(IUnitOfWork uow)
        {
            _uow = uow;
            _user = uow.Repository<User>();
        }

        public async Task<List<PermissionDto>> GetPermissionsByUserIdAsync(int userId)
        {
            var user = await _user
                .Include(u => u.Role)
                    .ThenInclude(r => r.RolePermissions)
                        .ThenInclude(rp => rp.Permission)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                throw new Exception("User not found");

            return user.Role.RolePermissions
                .Select(rp => new PermissionDto
                {
                    IsShow = rp.Permission.isShow,
                    IsAdd = rp.Permission.isAdd,
                    IsEdit = rp.Permission.isEdit,
                    IsDelete = rp.Permission.isDelete
                }).ToList();
        }
    }

}

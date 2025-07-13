using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationSystem.Application.IService.IAuth
{
    public interface IPermissionService
    {
        Task<List<PermissionDto>> GetPermissionsByUserIdAsync(int userId);
    }
}

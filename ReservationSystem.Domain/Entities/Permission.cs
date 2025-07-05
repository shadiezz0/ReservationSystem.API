using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationSystem.Domain.Entities
{
    public class Permission
    {
        public int Id { get; set; }
        public string Name { get; set; }

        // Many Permissions → Many Roles
        public ICollection<RolePermission> RolePermissions { get; set; }
    }

}

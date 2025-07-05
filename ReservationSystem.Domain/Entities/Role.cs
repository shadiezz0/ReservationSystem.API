using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationSystem.Domain.Entities
{
    public class Role
    {
        public int Id { get; set; }
        public string Name { get; set; }

        // One Role → Many Users
        public ICollection<User> Users { get; set; }

        // Role → many-to-many → Permissions
        public ICollection<RolePermission> RolePermissions { get; set; }
    }


}

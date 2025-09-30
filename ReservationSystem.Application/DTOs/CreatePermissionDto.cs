using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationSystem.Application.DTOs
{
    public class CreatePermissionDto
    {
        public ResourceType Resource { get; set; }
        public bool IsShow { get; set; }
        public bool IsAdd { get; set; }
        public bool IsEdit { get; set; }
        public bool IsDelete { get; set; }

        public int RoleId { get; set; } = 3;  // Assign permission directly to a role
    }
}

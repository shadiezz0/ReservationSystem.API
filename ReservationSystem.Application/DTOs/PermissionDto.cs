using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationSystem.Application.DTOs
{
    // CreatePermissionDto.cs
    public class CreatePermissionDto
    {
        public bool IsShow { get; set; }
        public bool IsAdd { get; set; }
        public bool IsEdit { get; set; }
        public bool IsDelete { get; set; }
    }

    // UpdatePermissionDto.cs
    public class UpdatePermissionDto : CreatePermissionDto
    {
        public int Id { get; set; }
    }

    public class AssignPermissionToRoleDto
    {
        public int RoleId { get; set; }
        public List<int> PermissionIds { get; set; }
    }
}

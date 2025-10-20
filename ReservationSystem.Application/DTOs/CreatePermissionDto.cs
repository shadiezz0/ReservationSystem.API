using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationSystem.Application.DTOs
{
    public class PermissionDto
    {
        public int Id { get; set; }
        public ResourceType Resource { get; set; }
        public bool IsShow { get; set; }
        public bool IsAdd { get; set; }
        public bool IsEdit { get; set; }
        public bool IsDelete { get; set; }
    }
    public class CreatePermissionDto: PermissionDto
    {
        public int RoleId { get; set; }   // Assign permission directly to a role
    }
    public class UpdateRole
    {
        public int RoleId { get; set; }   
        public int UserId { get; set; }
    }
    public class UpdateUserDto
    {
        public int UserId { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public int? RoleId { get; set; }
        public List<PermissionDto>? Permissions { get; set; }
    }
}

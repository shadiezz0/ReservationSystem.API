namespace ReservationSystem.Application.DTOs
{
      public class RegisterDto
      {
            public string FullName { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
            public int RoleId { get; set; } = (int)RoleType.User; // Default to User role

    }

      public class LoginDto
      {
            public string Email { get; set; }
            public string Password { get; set; }
      }

      public class TokenDto
      {
            public string AccessToken { get; set; }
            public string RefreshToken { get; set; }
      }

    public class CreateRoleDto
    {
        public string Name { get; set; }
        public RoleType RoleType { get; set; }
    }
    public class GetRoleDto : CreateRoleDto
    {
        public int Id { get; set; }

    }

    public class UpdateRolePermissionsDto
    {
        public List<int> PermissionIds { get; set; }
    }

}

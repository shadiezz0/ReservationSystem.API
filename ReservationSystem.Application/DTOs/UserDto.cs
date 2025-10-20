using static ReservationSystem.Domain.Constants.Enums;

namespace ReservationSystem.Application.DTOs
{
      public class RegisterDto
      {
            public string FullName { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
            public int RoleId { get; set; } = (int)RoleType.User;
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

    public class UserProfileDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public RoleType RoleType { get; set; }
        public int ReservationCount { get; set; }
    }

    public class UserWithPermissionsDto
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string RoleName { get; set; }
        public List<ListPermissionDto> Permissions { get; set; }
    }

    public class ListPermissionDto
    {
        public string Resource { get; set; }   // use string for name
        public bool IsShow { get; set; }
        public bool IsEdit { get; set; }
        public bool IsAdd { get; set; }
        public bool IsDelete { get; set; }
    }
}

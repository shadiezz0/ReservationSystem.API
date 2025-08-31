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
}

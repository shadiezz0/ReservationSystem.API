namespace ReservationSystem.Application.DTOs
{
      public class RegisterDto
      {
            public string FullName { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
            public int RoleId { get; set; } // Default to User role

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

namespace ReservationSystem.Application.DTOs
{
      public class RegisterDto
      {
            public string FullName { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
      }
      public class LoginDto
      {
            public string Email { get; set; }
            public string Password { get; set; }
      }
      public class ForgotPasswordDto
      {
            public string Email { get; set; }
      }

}

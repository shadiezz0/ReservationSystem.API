namespace ReservationSystem.Application.DTOs
{
      public class RegisterDto
      {
            public string FullName { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
            //public int RoleId { get; set; } = 2; // Default to User role

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
    public class PermissionDto
    {
        public int Id { get; set; }
        public string Resource { get; set; }
        public bool isShow { get; set; }
        public bool isAdd { get; set; }
        public bool isEdit { get; set; }
        public bool isDelete { get; set; }
    }


}

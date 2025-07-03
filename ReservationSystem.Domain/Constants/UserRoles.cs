namespace ReservationSystem.Domain.Constants
{
      public static class UserRoles
      {
            public const string SuperAdmin = "SuperAdmin";
            public const string Admin = "Admin";
            public const string User = "User";

            public static List<string> All = new() { SuperAdmin, Admin, User };
      }
}

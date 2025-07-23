namespace ReservationSystem.Domain.Constants
{
    public class Enums
    {
        public enum Result
        {
            Failed = 0,
            Success = 1,
            Exist = 2,
            NoDataFound = 3,
            NotExsit = 4,
            Unauthorized = 5,
        }

        public enum AlartType
        {
            information = 0,
            warning = 1,
            error = 2,
            success = 3,
            Unauthorized = 4,
        }

        public enum AlartShow
        {
            note = 0,
            popup = 1
        }

        public enum PermissionAction
        {
            Show,
            Add,
            Edit,
            Delete
        }

        public enum RoleType
        {
            SuperAdmin = 1,
            User = 2
        }

    }
}

namespace ReservationSystem.Application.IService
{
    public interface ICurrentUserService
    {
        int? GetCurrentUserId();
        string? GetCurrentUserRole();
        bool IsCurrentUserSuperAdmin();
        bool IsCurrentUserAdmin();
    }
}
namespace ReservationSystem.Application.IService
{
    public interface IAuthService
    {
        Task<ResponseResult> RegisterAsync(RegisterDto dto);
        Task<ResponseResult> LoginAsync(LoginDto dto);
        Task<ResponseResult> RefreshTokenAsync(TokenDto dto);
    }

}

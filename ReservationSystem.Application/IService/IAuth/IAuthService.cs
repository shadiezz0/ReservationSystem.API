
namespace ReservationSystem.Application.IService.IAuth
{
    public interface IAuthService
    {
        Task<ResponseResult> RegisterAsync(RegisterDto dto);
        Task<ResponseResult> LoginAsync(LoginDto dto);
        Task<ResponseResult> ForgotPasswordAsync(ForgotPasswordDto dto);
    }

}

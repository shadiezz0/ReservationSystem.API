

namespace ReservationSystem.API.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService) => _authService = authService;

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto) =>
                Ok(await _authService.RegisterAsync(dto));

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto) =>
                Ok(await _authService.LoginAsync(dto));

        // when access expires use this endpoint to get a new access token using the refresh token
        [AllowAnonymous]
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(TokenDto dto) =>
            Ok(await _authService.RefreshTokenAsync(dto));
    }

}

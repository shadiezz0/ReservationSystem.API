using Microsoft.AspNetCore.Mvc;
using ReservationSystem.Application.DTOs;
using ReservationSystem.Application.IService;

namespace ReservationSystem.API.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class AuthController : ControllerBase
      {
            private readonly IAuthService _authService;

            public AuthController(IAuthService authService) => _authService = authService;
       
        [HttpPost("register")]
            public async Task<IActionResult> Register(RegisterDto dto) =>
                Ok(await _authService.RegisterAsync(dto));
        
        [HttpPost("login")]
            public async Task<IActionResult> Login(LoginDto dto) =>
                Ok(await _authService.LoginAsync(dto));

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(TokenDto dto) => Ok(await _authService.RefreshTokenAsync(dto));
    }

}

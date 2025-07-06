using ReservationSystem.Application.IService.IResrvations;

namespace ReservationSystem.API.Controllers.User
{
    [ApiController]
    [Route("api/[controller]/user")]
    [Authorize(Roles = "User")]
    public class UserReservationController : ControllerBase
    {
        private readonly IReservationService _service;

        public UserReservationController(IReservationService service)
        {
            _service = service;
        }

        [HttpPost("CreateReservation")] public async Task<IActionResult> Create(CreateReservationDto dto) => Ok(await _service.CreateAsync(dto));
        [HttpGet("MyReservations/{userId}")] public async Task<IActionResult> MyReservations(int userId) => Ok(await _service.GetByUserIdAsync(userId));
    }

}

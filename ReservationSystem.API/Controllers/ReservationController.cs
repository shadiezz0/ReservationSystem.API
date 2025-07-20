using ReservationSystem.Application.IService;

namespace ReservationSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReservationController : ControllerBase
    {
        private readonly IReservationService _reservationService;

        public ReservationController(IReservationService reservationService)
        {
            _reservationService = reservationService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _reservationService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,SuperAdmin,User")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _reservationService.GetByIdAsync(id);
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> Create(CreateReservationDto dto)
        {
            var result = await _reservationService.CreateAsync(dto);
            return Ok(result);
        }

        [HttpPut]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> Update(UpdateReservationDto dto)
        {
            var result = await _reservationService.UpdateAsync(dto);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _reservationService.DeleteAsync(id);
            return Ok(result);
        }

        [HttpGet("user/{userId}")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> GetByUserId(int userId)
        {
            var result = await _reservationService.GetByUserIdAsync(userId);
            return Ok(result);
        }

        [HttpPut("confirm/{id}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> Confirm(int id)
        {
            var result = await _reservationService.ConfirmReservationAsync(id);
            return Ok(result);
        }

        [HttpPut("cancel/{id}")]
        [Authorize(Roles = "User,SuperAdmin,Admin")]
        public async Task<IActionResult> Cancel(int id)
        {
            var result = await _reservationService.CancelReservationAsync(id);
            return Ok(result);
        }

        [HttpPost("filter-by-date")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> FilterByDate(FilterReservationDto dto)
        {
            var result = await _reservationService.FilterByDateAsync(dto);
            return Ok(result);
        }

    }
}

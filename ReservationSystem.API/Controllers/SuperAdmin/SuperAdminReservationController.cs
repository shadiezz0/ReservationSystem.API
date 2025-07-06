using ReservationSystem.Application.IService.IResrvations;

namespace ReservationSystem.API.Controllers.SuperAdmin
{
    // Super Admin - Full Access
    [Route("api/[controller]/super")]
    [ApiController]
    [Authorize(Roles = "SuperAdmin")]
    public class SuperAdminReservationController : ControllerBase
    {
        private readonly IReservationService _service;

        public SuperAdminReservationController(IReservationService service)
        {
            _service = service;
        }
        [HttpPost("CreateReservation")] public async Task<IActionResult> Create(CreateReservationDto dto) => Ok(await _service.CreateAsync(dto));
        [HttpPut("UpdateReservation")] public async Task<IActionResult> Update(UpdateReservationDto dto) => Ok(await _service.UpdateAsync(dto));
        [HttpDelete("DeleteReservationBy/{id}")] public async Task<IActionResult> Delete(int id) => Ok(await _service.DeleteAsync(id));
        [HttpGet("GatAllReservation")] public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());
        [HttpGet("GetReservationById/{id}")] public async Task<IActionResult> GetById(int id) => Ok(await _service.GetByIdAsync(id));
    }
}

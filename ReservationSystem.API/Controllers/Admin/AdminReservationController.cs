using ReservationSystem.Application.IService.IResrvations;

namespace ReservationSystem.API.Controllers.Admin
{
    // Admin - Approve, Reject, View All
    [ApiController]
    [Route("api/[controller]/admin")]
    [Authorize(Roles = "Admin")]
    public class AdminReservationController : ControllerBase
    {
         private readonly IReservationService _service;

    public AdminReservationController(IReservationService service)
    {
        _service = service;
    }

    [HttpGet("GetAllReservation")] public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());
    [HttpGet("GetReservationById/{id}")] public async Task<IActionResult> GetById(int id) => Ok(await _service.GetByIdAsync(id));
    [HttpPut("confirm/{id}")] public async Task<IActionResult> Confirm(int id) => Ok(await _service.ConfirmReservationAsync(id));
    [HttpPut("cancel/{id}")] public async Task<IActionResult> Cancel(int id) => Ok(await _service.CancelReservationAsync(id));
    }
}

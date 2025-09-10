
namespace ReservationSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReservationController : ControllerBase
    {
        private readonly IReservationService _reservationService;
        private readonly IPermissionCheckerService _permissionCheckerService;


        public ReservationController(IReservationService reservationService, IPermissionCheckerService permissionCheckerService)
        {
            _reservationService = reservationService;
            _permissionCheckerService = permissionCheckerService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var permissionResult = await _permissionCheckerService.HasPermissionAsync(ResourceType.Reservations, PermissionAction.Show);
            if (permissionResult != null)
                return Ok(permissionResult);

            var result = await _reservationService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var permissionResult = await _permissionCheckerService.HasPermissionAsync(ResourceType.Reservations, PermissionAction.Show);
            if (permissionResult != null)
                return Ok(permissionResult);
            var result = await _reservationService.GetByIdAsync(id);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateReservationDto dto)
        {
            var permissionResult = await _permissionCheckerService.HasPermissionAsync(ResourceType.Reservations, PermissionAction.Add);
            if (permissionResult != null)
                return Ok(permissionResult);
            var result = await _reservationService.CreateAsync(dto);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(UpdateReservationDto dto,int id)
        {
            var permissionResult = await _permissionCheckerService.HasPermissionAsync(ResourceType.Reservations, PermissionAction.Edit);
            if (permissionResult != null)
                return Ok(permissionResult);
            dto.Id = id;
            var result = await _reservationService.UpdateAsync(dto);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var permissionResult = await _permissionCheckerService.HasPermissionAsync(ResourceType.Reservations, PermissionAction.Delete);
            if (permissionResult != null)
                return Ok(permissionResult);
            var result = await _reservationService.DeleteAsync(id);
            return Ok(result);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUserId(int userId)
        {
            var result = await _reservationService.GetByUserIdAsync(userId);
            return Ok(result);
        }

        [HttpPut("confirm/{id}")]
        public async Task<IActionResult> Confirm(int id)
        {
            var result = await _reservationService.ConfirmReservationAsync(id);
            return Ok(result);
        }

        [HttpPut("cancel/{id}")]
        public async Task<IActionResult> Cancel(int id)
        {
            var result = await _reservationService.CancelReservationAsync(id);
            return Ok(result);
        }

        [HttpGet("filter-by-date")]
        public async Task<IActionResult> FilterByDate([FromBody] FilterReservationDto dto)
        {
            var permissionResult = await _permissionCheckerService.HasPermissionAsync(ResourceType.Reservations, PermissionAction.Show);
            if (permissionResult != null)
                return Ok(permissionResult);
            var result = await _reservationService.FilterByDateAsync(dto);
            return Ok(result);
        }

    }
}

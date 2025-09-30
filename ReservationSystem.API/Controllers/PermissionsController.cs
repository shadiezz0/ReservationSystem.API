using ReservationSystem.Domain.Entities;


namespace ReservationSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PermissionsController : ControllerBase
    {

        private readonly IPermissionCheckerService _permissionService;

        public PermissionsController(IPermissionCheckerService permissionService)
        {
            _permissionService = permissionService;
        }
        [HttpPost("create")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> CreatePermission([FromBody] CreatePermissionDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _permissionService.CreatePermissionAsync(dto);
            if (result.Result == Result.Success)
                return Ok(new { Message = result.Alart.MessageEn });

            return Ok(new
            {
                Message = result.Alart.MessageEn,
                data = result.Data,
             
            });
            
        }
    }
}

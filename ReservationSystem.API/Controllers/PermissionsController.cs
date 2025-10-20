using ReservationSystem.Application.Service;
using ReservationSystem.Domain.Entities;


namespace ReservationSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PermissionsController : ControllerBase
    {

        private readonly IPermissionCheckerService _permissionService;
        private readonly IRoleService _roleService;
        private readonly IAuthService _authService;

        public PermissionsController(IPermissionCheckerService permissionService, IRoleService roleService, IAuthService authService)
        {
            _permissionService = permissionService;
            _roleService = roleService;
            _authService = authService;
        }
        [HttpPost("CreatePermission")]
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
        //[HttpPut("updateRole/{userId}")]
        //[Authorize(Roles = "SuperAdmin")]
        //public async Task<IActionResult> UpdateUserRole(int userId, [FromBody] int newRoleId)
        //{
        //    var permissionResult = await _permissionService.HasPermissionAsync(ResourceType.Users, PermissionAction.Edit);
        //    if (permissionResult != null)
        //        return Ok(permissionResult);
        //    var success = await _roleService.UpdateUserRoleAsync(userId, newRoleId);
        //    if (success.Result != Result.Success)
        //        return NotFound(new { MessageAr = success.Alart.MessageAr, MessageEn = success.Alart.MessageEn });

        //    return Ok(new { MessageAr = success.Alart.MessageAr , MessageEn = success.Alart.MessageEn , data = success.Data});
        //}
        [HttpPost("CreateUser")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> CreateUser([FromBody] RegisterDto dto)
        {
            var permissionResult = await _permissionService.HasPermissionAsync(ResourceType.Users, PermissionAction.Edit);
            if (permissionResult != null)
                return Ok(permissionResult);
            var success = await _authService.RegisterAsync(dto);
            if (success.Result != Result.Success)
                return NotFound(new { MessageAr = success.Alart.MessageAr, MessageEn = success.Alart.MessageEn });

            return Ok(new { MessageAr = success.Alart.MessageAr, MessageEn = success.Alart.MessageEn, data = success.Data });
        }
        [HttpPut("UpdateUser")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> UpdateUser(UpdateUserDto dto)
        {
            var permissionResult = await _permissionService.HasPermissionAsync(ResourceType.Users, PermissionAction.Edit);
            if (permissionResult != null)
                return Ok(permissionResult);
            var success = await _roleService.UpdateUserAsync(dto);
            if (success.Result != Result.Success)
                return NotFound(new { MessageAr = success.Alart.MessageAr, MessageEn = success.Alart.MessageEn });

            return Ok(new { MessageAr = success.Alart.MessageAr, MessageEn = success.Alart.MessageEn, data = success.Data });
        }
        [HttpGet("GetAllUsers")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> GetAllUsers()
        {
            var permissionResult = await _permissionService.HasPermissionAsync(ResourceType.Users, PermissionAction.Edit);
            if (permissionResult != null)
                return Ok(permissionResult);
            var success = await _roleService.GetAllUsersAsync();
            if (success.Result != Result.Success)
                return NotFound(new { MessageAr = success.Alart.MessageAr, MessageEn = success.Alart.MessageEn });

            return Ok(new { MessageAr = success.Alart.MessageAr, MessageEn = success.Alart.MessageEn, data = success.Data });
        }

        [HttpDelete("{userId}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            var permissionResult = await _permissionService.HasPermissionAsync(ResourceType.Users, PermissionAction.Edit);
            if (permissionResult != null)
                return Ok(permissionResult);
            var result = await _roleService.DeleteUserAsync(userId);
            return Ok(result);
        }

    }
}

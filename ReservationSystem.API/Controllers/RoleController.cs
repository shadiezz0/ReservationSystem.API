namespace ReservationSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;
        private readonly IPermissionCheckerService _permissionCheckerService;

        public RoleController(IRoleService roleService, IPermissionCheckerService permissionCheckerService)
        {
            _roleService = roleService;
            _permissionCheckerService = permissionCheckerService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var permissionResult = await _permissionCheckerService.HasPermissionAsync(ResourceType.Roles, PermissionAction.Show);
            if (permissionResult != null)
                return Ok(permissionResult);

            var result = await _roleService.GetAllRolesAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var permissionResult = await _permissionCheckerService.HasPermissionAsync(ResourceType.Roles, PermissionAction.Show);
            if (permissionResult != null)
                return Ok(permissionResult);

            var result = await _roleService.GetByIdAsync(id);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateRoleDto dto)
        {
            var permissionResult = await _permissionCheckerService.HasPermissionAsync(ResourceType.Roles, PermissionAction.Add);
            if (permissionResult != null)
                return Ok(permissionResult);

            var result = await _roleService.CreateRoleAsync(dto);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateRoleDto dto)
        {
            var permissionResult = await _permissionCheckerService.HasPermissionAsync(ResourceType.Roles, PermissionAction.Edit);
            if (permissionResult != null)
                return Ok(permissionResult);

            var result = await _roleService.UpdateRoleAsync(id, dto);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var permissionResult = await _permissionCheckerService.HasPermissionAsync(ResourceType.Roles, PermissionAction.Delete);
            if (permissionResult != null)
                return Ok(permissionResult);

            var result = await _roleService.DeleteRoleAsync(id);
            return Ok(result);
        }

        [HttpGet("{roleId}/permissions")]
        public async Task<IActionResult> GetPermissionsForRole(int roleId)
        {
            var permissionResult = await _permissionCheckerService.HasPermissionAsync(ResourceType.Roles, PermissionAction.Show);
            if (permissionResult != null)
                return Ok(permissionResult);

            var result = await _roleService.GetPermissionsForRoleAsync(roleId);
            return Ok(result);
        }

        [HttpPost("{roleId}/permissions")]
        public async Task<IActionResult> AssignPermissionsToRole(int roleId, [FromBody] List<int> permissionIds)
        {
            var permissionResult = await _permissionCheckerService.HasPermissionAsync(ResourceType.Roles, PermissionAction.Edit);
            if (permissionResult != null)
                return Ok(permissionResult);

            var result = await _roleService.AssignPermissionsToRoleAsync(roleId, permissionIds);
            return Ok(result);
        }

        [HttpGet("profile")]
        public async Task<IActionResult> GetCurrentUserProfile()
        {
            var permissionResult = await _permissionCheckerService.HasPermissionAsync(ResourceType.Users, PermissionAction.Show);
            if (permissionResult != null)
                return Ok(permissionResult);
            var result = await _roleService.GetCurrentUserProfileAsync();
            return Ok(result);
        }

    }
}
namespace ReservationSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemController : ControllerBase
    {
        private readonly IItemService _itemService;
        private readonly IPermissionCheckerService _permissionCheckerService;

        public ItemController(IItemService itemService, IPermissionCheckerService permissionCheckerService)
        {
            _itemService = itemService;
            _permissionCheckerService = permissionCheckerService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateItemDto dto)
        {
            var permissionResult = await _permissionCheckerService.HasPermissionAsync(ResourceType.Items, PermissionAction.Add);
            if (permissionResult != null)
                return Ok(permissionResult);

            var result = await _itemService.CreateAsync(dto);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var permissionResult = await _permissionCheckerService.HasPermissionAsync(ResourceType.Items, PermissionAction.Delete);
            if (permissionResult != null)
                return Ok(permissionResult);

            var result = await _itemService.DeleteAsync(id);
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateItemDto dto)
        {
            var permissionResult = await _permissionCheckerService.HasPermissionAsync(ResourceType.Items, PermissionAction.Edit);
            if (permissionResult != null)
                return Ok(permissionResult);

            var result = await _itemService.UpdateAsync(dto);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var permissionResult = await _permissionCheckerService.HasPermissionAsync(ResourceType.Items, PermissionAction.Show);
            if (permissionResult != null)
                return Ok(permissionResult);

            var result = await _itemService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var permissionResult = await _permissionCheckerService.HasPermissionAsync(ResourceType.Items, PermissionAction.Show);
            if (permissionResult != null)
                return Ok(permissionResult);

            var result = await _itemService.GetByIdAsync(id);
            return Ok(result);
        }
        [HttpGet("GetByAdminId")]
        public async Task<IActionResult> GetByAdminId()
        {
            var permissionResult = await _permissionCheckerService.HasPermissionAsync(ResourceType.Items, PermissionAction.Show);
            if (permissionResult != null)
                return Ok(permissionResult);

            var result = await _itemService.GetItemByAdminAsync();
            return Ok(result);
        }
        //[HttpGet("Available")]
        //[Authorize(Roles = "Admin,SuperAdmin,User")]
        //public async Task<IActionResult> GetAvailable()
        //{
        //    var result = await _itemService.FilterAvailableAsync();
        //    return Ok(result);
        //}

        [HttpGet("Type/{itemTypeId}")]
        public async Task<IActionResult> GetByType(int itemTypeId)
        {
            var permissionResult = await _permissionCheckerService.HasPermissionAsync(ResourceType.Items, PermissionAction.Show);
            if (permissionResult != null)
                return Ok(permissionResult);

            var result = await _itemService.FilterByTypeAsync(itemTypeId);
            return Ok(result);
        }
    }
}

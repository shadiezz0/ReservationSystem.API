namespace ReservationSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemTypeController : ControllerBase
    {
        private readonly IItemTypeService _service;
        private readonly IPermissionCheckerService _permissionCheckerService;

        public ItemTypeController(IItemTypeService service, IPermissionCheckerService permissionCheckerService)
        {
            _service = service;
            _permissionCheckerService = permissionCheckerService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var permissionResult = await _permissionCheckerService.HasPermissionAsync(ResourceType.ItemTypes, PermissionAction.Show);
            if (permissionResult != null)
                return Ok(permissionResult);

            return Ok(await _service.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var permissionResult = await _permissionCheckerService.HasPermissionAsync(ResourceType.ItemTypes, PermissionAction.Show);
            if (permissionResult != null)
                return Ok(permissionResult);

            return Ok(await _service.GetByIdAsync(id));
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateItemTypeDto dto)
        {
            var permissionResult = await _permissionCheckerService.HasPermissionAsync(ResourceType.ItemTypes, PermissionAction.Add);
            if (permissionResult != null)
                return Ok(permissionResult);

            return Ok(await _service.CreateAsync(dto));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(UpdateItemTypeDto dto)
        {
            var permissionResult = await _permissionCheckerService.HasPermissionAsync(ResourceType.ItemTypes, PermissionAction.Edit);
            if (permissionResult != null)
                return Ok(permissionResult);

            return Ok(await _service.UpdateAsync(dto));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var permissionResult = await _permissionCheckerService.HasPermissionAsync(ResourceType.ItemTypes, PermissionAction.Delete);
            if (permissionResult != null)
                return Ok(permissionResult);

            return Ok(await _service.DeleteAsync(id));
        }
    }
}

using ReservationSystem.Application.IService.IItemType;

namespace ReservationSystem.API.Controllers.SuperAdmin
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "SuperAdmin")]
    public class ItemTypeController : ControllerBase
    {
        private readonly IItemTypeService _service;
        public ItemTypeController(IItemTypeService service) => _service = service;

        [HttpPost("CreateItemType")] public async Task<IActionResult> Create(CreateItemTypeDto dto) => Ok(await _service.CreateAsync(dto));
        [HttpDelete("DeleteItemTypeBy/{id}")] public async Task<IActionResult> Delete(int id) => Ok(await _service.DeleteAsync(id));
        [HttpGet("GetAllItemType")] public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());
    }
}

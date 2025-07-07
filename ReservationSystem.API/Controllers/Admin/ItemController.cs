using ReservationSystem.Application.IService.IItems;

namespace ReservationSystem.API.Controllers.Admin
{
    [ApiController]
    [Route("api/[controller]/admin")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class ItemController : ControllerBase
    {
        private readonly IItemService _service;
        public ItemController(IItemService service) => _service = service;

        [HttpPost("CreateItem")] public async Task<IActionResult> Create(CreateItemDto dto) => Ok(await _service.CreateAsync(dto));
        [HttpPut("UpdateItem")] public async Task<IActionResult> Update(UpdateItemDto dto) => Ok(await _service.UpdateAsync(dto));
        [HttpDelete("DeleteItemBy/{id}")] public async Task<IActionResult> Delete(int id) => Ok(await _service.DeleteAsync(id));
        [HttpGet("GetAllItem")] public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());
        [HttpGet("GetItemById/{id}")] public async Task<IActionResult> GetById(int id) => Ok(await _service.GetByIdAsync(id));
        [HttpGet("GetAvailableItem")] public async Task<IActionResult> GetAvailable() => Ok(await _service.FilterAvailableAsync());
        [HttpGet("GetItemTypeBy/{itemTypeId}")] public async Task<IActionResult> GetByType(int itemTypeId) => Ok(await _service.FilterByTypeAsync(itemTypeId));
    }
}

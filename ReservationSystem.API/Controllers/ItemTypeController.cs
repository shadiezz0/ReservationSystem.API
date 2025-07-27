namespace ReservationSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class ItemTypeController : ControllerBase
    {
        private readonly IItemTypeService _service;
        public ItemTypeController(IItemTypeService service)
        {
            _service = service;
        }


        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id) => Ok(await _service.GetByIdAsync(id));

        [HttpPost]
        public async Task<IActionResult> Create(CreateItemTypeDto dto) => Ok(await _service.CreateAsync(dto));

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(UpdateItemTypeDto dto) => Ok(await _service.UpdateAsync(dto));

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id) => Ok(await _service.DeleteAsync(id));

    }
}

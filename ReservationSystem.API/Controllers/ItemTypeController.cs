using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ReservationSystem.Application.IService;

namespace ReservationSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Ensure all endpoints require authentication
    public class ItemTypeController : ControllerBase
    {
        private readonly IItemTypeService _service;
        public ItemTypeController(IItemTypeService service)
        {
            _service = service;
        }


        [HttpGet]
        [Authorize(Roles = "Admin,SuperAdmin,User")]
        public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,SuperAdmin,User")]
        public async Task<IActionResult> Get(int id) => Ok(await _service.GetByIdAsync(id));

        [HttpPost]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Create(CreateItemTypeDto dto) => Ok(await _service.CreateAsync(dto));

        [HttpPut("{id}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Update(UpdateItemTypeDto dto) => Ok(await _service.UpdateAsync(dto));

        [HttpDelete("{id}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Delete(int id) => Ok(await _service.DeleteAsync(id));





    }
}

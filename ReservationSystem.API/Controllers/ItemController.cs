using Microsoft.AspNetCore.Mvc;
using ReservationSystem.Application.IService;

namespace ReservationSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemController : Controller
    {
        private readonly IItemService _itemService;
        public ItemController(IItemService itemService)
        {
            _itemService = itemService;
        }

        [HttpPost]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Create([FromBody] CreateItemDto dto)
        {
            var result = await _itemService.CreateAsync(dto);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _itemService.DeleteAsync(id);
            return Ok(result);
        }

        [HttpPut]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> Update([FromBody] UpdateItemDto dto)
        {
            var result = await _itemService.UpdateAsync(dto);
            return Ok(result);
        }

        [HttpGet]
        [Authorize(Roles = "Admin,SuperAdmin,User")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _itemService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,SuperAdmin,User")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _itemService.GetByIdAsync(id);
            return Ok(result);
        }

        [HttpGet("Available")]
        [Authorize(Roles = "Admin,SuperAdmin,User")]
        public async Task<IActionResult> GetAvailable()
        {
            var result = await _itemService.FilterAvailableAsync();
            return Ok(result);
        }

        [HttpGet("Type/{itemTypeId}")]
        [Authorize(Roles = "Admin,SuperAdmin,User")]
        public async Task<IActionResult> GetByType(int itemTypeId)
        {
            var result = await _itemService.FilterByTypeAsync(itemTypeId);
            return Ok(result);
        }





    }
}

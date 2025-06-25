using Microsoft.AspNetCore.Mvc;
using ReservationSystem.Application.DTOs;
using ReservationSystem.Application.Interfaces;

namespace ReservationSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationItemsController : ControllerBase
    {
        private readonly IReservationItemService _service;

        public ReservationItemsController(IReservationItemService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _service.GetByIdAsync(id);
            return item == null ? NotFound() : Ok(item);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ReservationItemDto dto)
        {
            await _service.AddAsync(dto);
            return Ok("Created");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ReservationItemDto dto)
        {
            await _service.UpdateAsync(id, dto);
            return Ok("Updated");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return Ok("Deleted");
        }
    }
}

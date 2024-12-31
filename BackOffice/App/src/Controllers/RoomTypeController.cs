using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sempi5.Domain.RoomEntity;
using Sempi5.Domain.RoomTypeEntity;

namespace Sempi5.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomTypeController : ControllerBase
    {
        private readonly RoomTypeService _service;

        public RoomTypeController(RoomTypeService service)
        {
            _service = service;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]  
        public async Task<ActionResult<RoomTypeDTO>> AddRoomType([FromBody] RoomTypeDTO newRoomTypeDto)
        {
            if (newRoomTypeDto == null || string.IsNullOrWhiteSpace(newRoomTypeDto.Name))
            {
                return BadRequest("Room type data is invalid.");
            }

            var result = await _service.AddRoomType(newRoomTypeDto);

            if (result == null)
            {
                return Conflict("Room type already exists.");
            }

            return Ok(result);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]  
        public async Task<ActionResult<IEnumerable<RoomTypeDTO>>> GetAllRoomTypes()
        {
            var roomTypes = await _service.GetAllRoomTypes();
            return Ok(roomTypes);
        }
    }
}

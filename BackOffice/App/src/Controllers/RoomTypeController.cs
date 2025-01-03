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

        /**
         * Handles POST request to add a new room type.
         * @param newRoomTypeDto RoomTypeDTO - The data transfer object containing the details of the new room type.
         * @return Task<ActionResult<RoomTypeDTO>> - The result of the add operation, either a BadRequest if the data is invalid, a Conflict if the room type already exists, or an Ok with the added room type.
         * @author Ricardo Guimarães
         * @date 10/12/2024
         */
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

         /**
         * Handles GET request to retrieve all room types.
         * @return Task<ActionResult<IEnumerable<RoomTypeDTO>>> - The result of the get operation, either a BadRequest if the operation fails or an Ok with the list of room types.
         * @author Ricardo Guimarães
         * @date 10/12/2024
         */
        [HttpGet]
        [Authorize(Roles = "Admin")]  
        public async Task<ActionResult<IEnumerable<RoomTypeDTO>>> GetAllRoomTypes()
        {
            var roomTypes = await _service.GetAllRoomTypes();
            return Ok(roomTypes);
        }
    }
}

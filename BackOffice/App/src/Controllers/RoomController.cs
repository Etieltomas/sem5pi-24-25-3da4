using Microsoft.AspNetCore.Mvc;
using Sempi5.Domain.RoomEntity;


namespace Sempi5.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomController : ControllerBase
    {

        private readonly RoomService _service;

        public RoomController(RoomService service)
        {
            _service = service;
        }

        /// <summary>
        /// Retrieves all rooms in the system.
        /// @actor: Tomás Leite
        /// @date: 30/11/2024
        /// </summary>
        /// <returns>List of all rooms.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoomDTO>>> GetAllRooms()
        {
            return Ok(await _service.GetAllRooms());
        }

        /// <summary>
        /// Retrieves a specific room by its ID.
        /// @actor: Tomás Leite
        /// @date: 30/11/2024
        /// </summary>
        /// <param name="id">The ID of the room to retrieve.</param>
        /// <returns>Room details or a NotFound result.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<RoomDTO>> GetRoom(long id)
        {
            var room = await _service.GetRoomByID(new RoomID(id));

            if (room == null)
            {
                return NotFound();
            }

            return Ok(room);
        }

        /// <summary>
        /// Retrieves the operating status of a specific room by its ID.
        /// @actor: Tomás Leite
        /// @date: 30/11/2024
        /// </summary>
        /// <param name="id">The ID of the room to retrieve the operating status for.</param>
        /// <returns>Operating status or a NotFound result.</returns>
        [HttpGet("operating-status/{id}")]
        public async Task<ActionResult<string>> GetRoomOperatingStatus(long id)
        {
            var status = await _service.GetRoomOperatingStatus(new RoomID(id));

            if (status == null)
            {
                return NotFound();
            }

            return Ok(status);
        }
    }
}

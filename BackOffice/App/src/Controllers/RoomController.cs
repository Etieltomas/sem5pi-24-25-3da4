using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sempi5.Domain.PatientEntity;
using Sempi5.Domain.RoomEntity;
using Sempi5.Domain.TokenEntity;


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


        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoomDTO>>> GetAllRooms()
        {
            return Ok(await _service.GetAllRooms());
        }

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

        [HttpGet("operating-status/{id}")]
        public async Task<ActionResult<string>> GetRoomOperatingStatus(long id){
            var status = await _service.GetRoomOperatingStatus(new RoomID(id));
            Console.WriteLine(status);
            if (status == null)
            {
                return NotFound();
            }

            return Ok(status);
        }
    }
}
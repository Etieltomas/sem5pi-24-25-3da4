using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sempi5.Domain.User;
using Sempi5.Infrastructure.Databases;


namespace Sempi5.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class SystemUserController : ControllerBase
    {

        private readonly SystemUserService _service;

        public SystemUserController(SystemUserService service)
        {
            _service = service;
        }

        // Function to create SystemUser
        [HttpPost("register")]
        public async Task<ActionResult<SystemUserDTO>> RegisterUser(SystemUserDTO SystemUserDTO)
        {
            var SystemUser = await _service.AddUser(SystemUserDTO);

            return CreatedAtAction(nameof(GetUser), new { id = SystemUser.Id }, SystemUser);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SystemUserDTO>>> GetAllUsers()
        {
            return Ok(await _service.GetAllSystemUsers());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SystemUserDTO>> GetUser(long id)
        {
            var SystemUser = await _service.GetSystemUser(new SystemUserId(id));

            if (SystemUser == null)
            {
                return NotFound();
            }

            return Ok(SystemUser);
        }

        [HttpGet("email/{email}")]
        public async Task<ActionResult<SystemUserDTO>> GetUserByEmail(string email)
        {
            var SystemUser = await _service.GetUserByEmail(email);

            if (SystemUser == null)
            {
                return NotFound();
            }

            return Ok(SystemUser);
        }
    }
}
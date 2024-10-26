using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sempi5.Domain.TokenEntity;
using Sempi5.Domain.UserEntity;
using Sempi5.Infrastructure.Databases;


namespace Sempi5.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SystemUserController : ControllerBase
    {

        private readonly SystemUserService _service;
        private readonly ILogger<SystemUserController> _logger;

        public SystemUserController(SystemUserService service, ILogger<SystemUserController> logger)
        {
            _service = service;
            _logger = logger;
        }


        [HttpGet("confirm/{token}/{active}")]
        public async Task<IActionResult> Update(Guid token, bool active)
        {
            var user = await _service.Update(token, active);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(new { active = user.Active });
        }


        [HttpPut("active/{email}/{active}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateActive(string email, bool active)
        {
            var user = await _service.UpdateActive(email, active);

            if (user == null)
            {
                return NotFound();
            }

            _logger.LogInformation("User {email} is now {active}", email, active);

            return Ok(new { active = user.Active });
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
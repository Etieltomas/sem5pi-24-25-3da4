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

        /// <summary>
        /// Updates the active status of a user by token.
        /// @actor: Tomás Leite
        /// @date: 30/11/2024
        /// </summary>
        /// <param name="token">The token used to identify the user.</param>
        /// <param name="active">The active status to set.</param>
        /// <returns>Returns the updated active status or NotFound if the user does not exist.</returns>
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

        /// <summary>
        /// Updates the active status of a user by email. Only accessible by Admin.
        /// @actor: Tomás Leite
        /// @date: 30/11/2024
        /// </summary>
        /// <param name="email">The email of the user.</param>
        /// <param name="active">The active status to set.</param>
        /// <returns>Returns the updated active status or NotFound if the user does not exist.</returns>
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

        /// <summary>
        /// Registers a new system user.
        /// @actor: Tomás Leite
        /// @date: 30/11/2024
        /// </summary>
        /// <param name="SystemUserDTO">The data transfer object representing the system user.</param>
        /// <returns>Returns the created system user.</returns>
        [HttpPost("register")]
        public async Task<ActionResult<SystemUserDTO>> RegisterUser(SystemUserDTO SystemUserDTO)
        {
            var SystemUser = await _service.AddUser(SystemUserDTO);

            return CreatedAtAction(nameof(GetUser), new { id = SystemUser.Id }, SystemUser);
        }

        /// <summary>
        /// Retrieves all system users.
        /// @actor: Tomás Leite
        /// @date: 30/11/2024
        /// </summary>
        /// <returns>A list of all system users.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SystemUserDTO>>> GetAllUsers()
        {
            return Ok(await _service.GetAllSystemUsers());
        }

        /// <summary>
        /// Retrieves a system user by ID.
        /// @actor: Tomás Leite
        /// @date: 30/11/2024
        /// </summary>
        /// <param name="id">The ID of the system user.</param>
        /// <returns>The system user or NotFound if the user does not exist.</returns>
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

        /// <summary>
        /// Retrieves a system user by their email.
        /// @actor: Tomás Leite
        /// @date: 30/11/2024
        /// </summary>
        /// <param name="email">The email of the system user.</param>
        /// <returns>The system user or NotFound if the user does not exist.</returns>
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

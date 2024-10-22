using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using Sempi5.Domain.Staff;
using Sempi5.Infrastructure.Databases;


namespace Sempi5.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StaffController : ControllerBase
    {

        private readonly StaffService _service;

        public StaffController(StaffService service)
        {
            _service = service;
        }

        // Function to create staff
        [HttpPost("register")]
        [Authorize]
        public async Task<ActionResult<StaffDTO>> RegisterStaff(StaffDTO staffDTO)
        {
            var staff = await _service.AddStaffMember(staffDTO);

            // TODO: Send email for user to confirm it
            //          Maybe call the SystemUser service to create the user
            //           And call the Email Service to send the email
            return CreatedAtAction(nameof(GetStaffMember), new { id = staff.Id }, staff);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<StaffDTO>>> GetAllStaffMembers()
        {
            return await _service.GetAllStaffMembers();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<StaffDTO>> GetStaffMember(string id)
        {
            var staff = await _service.GetStaffMember(new StaffID(id));

            if (staff == null)
            {
                return NotFound();
            }

            return Ok(staff);
        }

    
    }
}
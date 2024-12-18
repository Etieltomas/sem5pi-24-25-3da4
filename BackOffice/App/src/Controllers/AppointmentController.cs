using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sempi5.Domain.AppointmentEntity;


namespace Sempi5.Controllers 
{
    [Route("api/[controller]")]
    [ApiController] 
    public class AppointmentController : ControllerBase
    {

        private readonly AppointmentService _service;

        public AppointmentController(AppointmentService service)
        {
            _service = service;
        }

        [HttpGet("doctor/{doctorEmail}")]
        [Authorize(Roles = "Staff, Doctor")]
        public async Task<IActionResult> GetAppointmentsByDoctor(string doctorEmail)
        {

            var list = await _service.GetAppointmentsByDoctor(doctorEmail);
            if (list == null)
            {
                return NotFound();
            }

            return Ok(list);
        }

        [HttpPatch("edit/{id}")]
        [Authorize(Roles = "Staff, Doctor")]
        public async Task<IActionResult> EditAppointment([FromBody] AppointmentDTO appointmentDTO, long id)
        {

            var appointment = await _service.EditAppointment(appointmentDTO, id);
            if (appointment == null)
            {
                return StatusCode(501);
            }

            return Ok(appointment);
        }
    }
}
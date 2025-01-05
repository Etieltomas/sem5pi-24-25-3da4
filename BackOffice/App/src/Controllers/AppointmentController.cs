using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sempi5.Domain.AppointmentEntity;


namespace Sempi5.Controllers 
{
    /// <summary>
    /// @author Simão Lopes
    /// @date 3/12/2024
    /// This controller provides API endpoints for managing appointments. 
    /// It includes methods to retrieve appointments by doctor, edit existing appointments, 
    /// and create new appointments. Authorization is required for all operations, restricted 
    /// to staff and doctors.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController] 
    public class AppointmentController : ControllerBase
    {

        private readonly AppointmentService _service;
        private readonly ILogger<OperationRequestController> _logger;

        public AppointmentController(AppointmentService service, ILogger<OperationRequestController> logger)
        {
            _service = service;
            _logger = logger;
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

        [HttpPost("create")]
        [Authorize(Roles = "Staff, Doctor")]
        public async Task<IActionResult> CreateAppointment([FromBody] AppointmentDTO appointmentDTO)
        {

            var appointment = await _service.CreateAppointment(appointmentDTO);
            if (appointment == null)
            {
                return StatusCode(501);
            }

            return Ok(appointment);
        }
    }
}
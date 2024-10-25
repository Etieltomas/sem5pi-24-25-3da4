using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sempi5.Domain.PatientEntity;
using Sempi5.Infrastructure.Databases;


namespace Sempi5.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientController : ControllerBase
    {

        private readonly PatientService _service;

        public PatientController(PatientService service)
        {
            _service = service;
        }

        [HttpPut("associate/{email}")]
        [Authorize]
        public async Task<IActionResult> AssociateAccount(string email)
        {
            var cookie = User.Identity as ClaimsIdentity;
            var emailCookie = cookie?.FindFirst(ClaimTypes.Email)?.Value;

            var patient = await _service.AssociateAccount(email, emailCookie);
            
            if (patient == null)
            {
                return BadRequest();
            }

            return Ok(new { sucess = true });
        }

        // Function to create patient
        [HttpPost("register")]
        public async Task<ActionResult<PatientDTO>> RegisterPatient(PatientDTO PatientDTO)
        {
            var patient = await _service.AddPatient(PatientDTO);

            return CreatedAtAction(nameof(GetPatient), new { id = patient.MedicalRecordNumber }, patient);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PatientDTO>>> GetAllPatients()
        {
            return Ok(await _service.GetAllPatients());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PatientDTO>> GetPatient(string id)
        {
            var patient = await _service.GetPatientByMedicalRecordNumber(new PatientID(id));

            if (patient == null)
            {
                return NotFound();
            }

            return Ok(patient);
        }

        [HttpGet("email/{email}")]
        public async Task<ActionResult<PatientDTO>> GetPatientByEmail(string email)
        {
            var patient = await _service.GetPatientByEmail(email);

            if (patient == null)
            {
                return NotFound();
            }

            return Ok(patient);
        }

        [HttpGet("name/{name}")]
        public async Task<ActionResult<PatientDTO>> GetPatientByName(string name)
        {
            var patient = await _service.GetPatientByName(name);

            if (patient == null)
            {
                return NotFound();
            }
            
            return Ok(patient);
        }

        [HttpGet("search")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<PatientDTO>>> SearchPatients(
            [FromQuery] string? name,
            [FromQuery] string? email,
            [FromQuery] string? dateOfBirth,
            [FromQuery] string? medicalRecordNumber,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10
        )
        {
            var patients = await _service.SearchPatients(name, email, dateOfBirth, medicalRecordNumber, page, pageSize);

            if (patients == null || !patients.Any())
            {
                return NotFound("No patients found with the given criteria");
            }

            return Ok(patients);
        }
    }
}
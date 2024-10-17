using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sempi5.Domain.Patient;
using Sempi5.Infrastructure.Databases;


namespace Sempi5.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PatientController : ControllerBase
    {

        private readonly PatientService _service;

        public PatientController(PatientService service)
        {
            _service = service;
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
    }
}
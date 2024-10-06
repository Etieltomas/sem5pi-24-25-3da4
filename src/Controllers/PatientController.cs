using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sempi5.Domain.Patient;
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

        // Function to create patient
        [HttpPost]
        public async Task<ActionResult<PatientDTO>> CreatePatient(PatientDTO PatientDTO)
        {
            var patient = await _service.AddPatient(PatientDTO);

            return CreatedAtAction(nameof(GetPatient), new { id = patient.MedicalRecordNumber }, patient);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PatientDTO>>> GetAllPatients()
        {
            return await _service.GetAllPatients();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PatientDTO>> GetPatient(long id)
        {
            var patient = await _service.GetPatient(id);

            if (patient == null)
            {
                return NotFound();
            }

            return Ok(patient);
        }

    }
}
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sempi5.Domain.MedicalRecordEntity;


namespace Sempi5.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MedicalRecordController : ControllerBase
    {

        private readonly MedicalRecordService _service;
        private readonly ILogger<MedicalRecordController> _logger;

        public MedicalRecordController(MedicalRecordService service, ILogger<MedicalRecordController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet("{patientEmail}")]
        [Authorize(Roles = "Patient")]
        public async Task<ActionResult<List<MedicalRecordDTO>>> GetMedicalRecord(string patientEmail)
        {
            var result = await _service.GetMedicalRecord(patientEmail);
            if (result == null)
            {
                return BadRequest();
            }

            return Ok(result);
        }

        [HttpGet("entry/{idMedicalRecord}")]
        [Authorize(Roles = "Staff, Doctor")]
        public async Task<ActionResult<List<MedicalRecordDTO>>> GetEntryMedicalRecord(string idMedicalRecord)
        {
            var result = await _service.GetEntryMedicalRecord(idMedicalRecord);
            if (result == null)
            {
                return BadRequest();
            }

            return Ok(result);
        }
        
        [HttpGet]
        [Authorize(Roles = "Staff, Doctor")]
        public async Task<ActionResult<List<MedicalRecordDTO>>> GetAllMedicalRecords()
        {
            return Ok(await _service.GetAllMedicalRecords());
        }

        [HttpGet("search")]
        [Authorize(Roles = "Staff, Doctor")]
        public async Task<ActionResult<List<MedicalRecordDTO>>> Search(
            [FromQuery] string? filter,
            [FromQuery] string patient,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10
        )
        {
            var result = await _service.Search(filter, patient, page, pageSize);
            if (result == null)
            {
                return BadRequest();
            }

            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Doctor")]
        public async Task<ActionResult<MedicalRecordDTO>> AddMedicalRecord([FromBody] MedicalRecordDTO medicalRecord)
        {
            var result = await _service.AddMedicalRecord(medicalRecord);
            if (result == null)
            {
                return BadRequest();
            }

            return Ok(result);
        }

        /*[HttpPatch("entry/{idMedicalRecord}/update")]
        [Authorize(Roles = "Staff, Doctor")]
        public async Task<ActionResult<RecordLineDTO>> Update(string idMedicalRecord, [FromBody] RecordLineDTO medicalRecordLine)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _service.Update(medicalRecordLine, idMedicalRecord);
            if (result == null)
            {
                return BadRequest();
            }
            return Ok(result);
        }*/

        [HttpPatch("entry/{idMedicalRecord}/update")]
        [Authorize(Roles = "Staff, Doctor")]
        public async Task<ActionResult<MedicalRecordDTO>> Update(string idMedicalRecord, [FromBody] MedicalRecordDTO medicalRecord)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _service.Update(medicalRecord, idMedicalRecord);
            if (result == null)
            {
                return BadRequest();
            }
            return Ok(result);
        }
    }
}
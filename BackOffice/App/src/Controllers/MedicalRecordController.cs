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

        public MedicalRecordController(MedicalRecordService service)
        {
            _service = service;
        }

        [HttpGet]
        [Authorize(Roles = "Staff")]
        public async Task<ActionResult<List<MedicalRecordDTO>>> GetAllMedicalRecords()
        {
            return Ok(await _service.GetAllMedicalRecords());
        }

        [HttpGet("search")]
        [Authorize(Roles = "Staff")]
        public async Task<ActionResult<List<MedicalRecordDTO>>> Search(
            [FromQuery] string? allergy,
            [FromQuery] string? condition,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10
        )
        {
            var result = await _service.Search(allergy, condition, page, pageSize);
            if (result == null)
            {
                return BadRequest();
            }

            return Ok(result);
        }
    }
}
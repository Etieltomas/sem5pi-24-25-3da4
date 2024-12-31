using Microsoft.AspNetCore.Mvc;
using Sempi5.Domain.MedicalConditionEntity;

namespace Sempi5.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MedicalConditionController : ControllerBase
    {
        private readonly MedicalConditionService _service;

        public MedicalConditionController(MedicalConditionService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<ActionResult<MedicalConditionDTO>> AddMedicalCondition([FromBody] MedicalConditionDTO medicalCondition)
        {
            var result = await _service.AddMedicalCondition(medicalCondition);
            if (result == null)
            {
                return BadRequest();
            }

            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult<List<MedicalConditionDTO>>> GetAllMedicalConditions()
        {
            return Ok(await _service.GetAllMedicalConditions());
        }
    }
}
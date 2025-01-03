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

        /**
         * Handles POST request to add a new medical condition.
         * @param medicalCondition - The MedicalConditionDTO object containing the details of the medical condition to be added.
         * @return ActionResult<MedicalConditionDTO> - The result of the add operation, either a BadRequest if the operation fails or an Ok with the added medical condition.
         * @author Ricardo Guimarães
         * @date 10/12/2024
         */
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

        /**
         * Handles GET request to get all medical conditions.
         * @return ActionResult<List<MedicalConditionDTO>> - The result of the get operation, either a BadRequest if the operation fails or an Ok with the list of medical conditions.
         * @author Ricardo Guimarães
         * @date 10/12/2024
         */
        [HttpGet]
        public async Task<ActionResult<List<MedicalConditionDTO>>> GetAllMedicalConditions()
        {
            return Ok(await _service.GetAllMedicalConditions());
        }
    }
}
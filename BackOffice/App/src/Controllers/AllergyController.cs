using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sempi5.Domain.AllergyEntity;
using Sempi5.Domain.AllergyEntityEntity;


namespace Sempi5.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AllergyController : ControllerBase
    {

        private readonly AllergyService _service;

        public AllergyController(AllergyService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<ActionResult<AllergyDTO>> AddAllergy([FromBody] AllergyDTO allergy)
        {
            return Ok(await _service.AddAllergy(allergy));
        }

        [HttpGet]
        public async Task<ActionResult<List<AllergyDTO>>> GetAllAllergies()
        {
            return Ok(await _service.GetAllAllergies());
        }
    }
}
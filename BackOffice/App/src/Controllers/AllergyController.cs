using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sempi5.Domain.AllergyEntity;

// @author Tomás Leite
// @date 1/12/2024
// @description This controller handles API endpoints for managing allergies. 
// It provides functionality to add a new allergy and retrieve all existing allergies. 
// Authorization is required for certain operations, such as adding an allergy.

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
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<AllergyDTO>> AddAllergy([FromBody] AllergyDTO allergy)
        {
            var result = await _service.AddAllergy(allergy);
            if (result == null)
            {
                return BadRequest();
            }

            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult<List<AllergyDTO>>> GetAllAllergies()
        {
            return Ok(await _service.GetAllAllergies());
        }
    }
}
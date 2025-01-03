using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sempi5.Domain.SpecializationEntity;

namespace Sempi5.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SpecializationController : ControllerBase
    {
        private readonly SpecializationService _service;

        public SpecializationController(SpecializationService service)
        {
            _service = service;
        }

        /**
         * Handles GET request to retrieve all specializations.
         * @return Task<ActionResult<IEnumerable<string>>> - The result of the get operation, either a NotFound if no specializations are found or     * Handles GET request to retrieve all specializations.
         * @return Task<ActionResult<IEnumerable<string>>> - The result of the get operation, either a NotFound if no specializations are found or an Ok with the list of specializations.
         * @author Ricardo Guimarães
         * @date 10/12/2024
         */
        [HttpGet]
        [Authorize(Roles = "Admin")]  
        public async Task<ActionResult<IEnumerable<string>>> GetAllSpecializations()
        {
            return Ok(await _service.GetAllSpecializations());
        }

        /**
         * Handles POST request to add a new specialization.
         * @param dto SpecializationCreateDTO - The data transfer object containing the details of the new specialization.
         * @return Task<ActionResult<string>> - The result of the add operation, either a BadRequest if the data is invalid or an Ok with the ID of the added specialization.
         * @author Ricardo Guimarães
         * @date 10/12/2024
         */
        [HttpPost]
        [Authorize(Roles = "Admin")]  
        public async Task<ActionResult<string>> AddSpecialization([FromBody] SpecializationCreateDTO dto)
        {
            if (string.IsNullOrEmpty(dto.Name) || string.IsNullOrEmpty(dto.Code))
            {
                return BadRequest("Specialization name and code are required.");
            }

            var specializationId = await _service.AddSpecialization(dto.Name, dto.Code, dto.Description);
            return CreatedAtAction(nameof(GetSpecializationById), new { id = specializationId }, specializationId);
        }

        /**
        * Handles GET request to retrieve a specialization by ID.
        * @param id long - The ID of the specialization to retrieve.
        * @return Task<ActionResult<string>> - The result of the get operation, either a NotFound if the specialization is not found or an Ok with the specialization details.
        * @author Ricardo Guimarães
        * @date 10/12/2024
        */
        [HttpGet("{id:long}")]
        [Authorize(Roles = "Admin")]  
        public async Task<ActionResult<string>> GetSpecializationById(long id)
        {
            var specialization = await _service.GetSpecializationById(id);
            if (specialization == null)
            {
                return NotFound("Specialization not found.");
            }
            return Ok(specialization);
        }

        /**
        * Handles Patch request to update an existing specialization.
        * @param id long - The ID of the specialization to update.
        * @param dto SpecializationUpdateDTO - The data transfer object containing the updated details of the specialization.
        * @return Task<IActionResult> - The result of the update operation, either a NotFound if the specialization is not found or a NoContent if the update is successful.
        * @author Ricardo Guimarães
        * @date 10/12/2024
        */
        [HttpPatch("update/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateSpecialization(long id, [FromBody] SpecializationUpdateDTO dto)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid specialization ID.");
            }

            var updated = await _service.UpdateSpecialization(id, dto);
            if (!updated)
            {
                return NotFound("Specialization not found.");
            }

            return NoContent();
        }

        /**
        * Handles GET request to search for specializations.
        * @param name string - The name of the specialization to search for.
        * @param code string - The code of the specialization to search for.
        * @param description string - The description of the specialization to search for.
        * @param page int - The page number to retrieve.
        * @param pageSize int - The number of items per page.
        * @return Task<ActionResult> - The result of the search operation, either a NotFound if no specializations are found or an Ok with the list of specializations.
        * @author Ricardo Guimarães
        * @date 10/12/2024
        */
        [HttpGet("list")]
        [Authorize(Roles = "Admin")]  
        public async Task<IActionResult> SearchSpecializations(
            [FromQuery] string? name,
            [FromQuery] string? code,
            [FromQuery] string? description,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10
        )
        {
            var spec = await _service.SearchSpecializations(name, code, description, page, pageSize);

            if (spec == null || spec.Count == 0)
            {
                return NotFound("No specializations found with the given criteria.");
            }

            return Ok(spec);
        }
    }
}

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

        // 7.2.12: Listar todas as especializações
        [HttpGet]
        public async Task<ActionResult<IEnumerable<string>>> GetAllSpecializations()
        {
            return Ok(await _service.GetAllSpecializations());
        }

        // 7.2.11: Adicionar nova especialização
        [HttpPost]
        public async Task<ActionResult<string>> AddSpecialization([FromBody] SpecializationCreateDTO dto)
        {
            if (string.IsNullOrEmpty(dto.Name) || string.IsNullOrEmpty(dto.Code))
            {
                return BadRequest("Specialization name and code are required.");
            }

            var specializationId = await _service.AddSpecialization(dto.Name, dto.Code, dto.Description);
            return CreatedAtAction(nameof(GetSpecializationById), new { id = specializationId }, specializationId);
        }

        // 7.2.12: Buscar especialização por ID
        [HttpGet("{id:long}")]
        public async Task<ActionResult<string>> GetSpecializationById(long id)
        {
            var specialization = await _service.GetSpecializationById(id);
            if (specialization == null)
            {
                return NotFound("Specialization not found.");
            }
            return Ok(specialization);
        }

        // 7.2.13: Atualizar uma especialização
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

        // 7.2.15: Obter lista de especializações
        [HttpGet("list")]
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

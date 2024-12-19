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
        [HttpPut("{id:long}")]
        public async Task<IActionResult> UpdateSpecialization(long id, [FromBody] SpecializationUpdateDTO dto)
        {
            if (string.IsNullOrEmpty(dto.Name) || string.IsNullOrEmpty(dto.Code))
            {
                return BadRequest("Specialization name and code are required.");
            }

            var updated = await _service.UpdateSpecialization(id, dto.Name, dto.Code, dto.Description);
            if (!updated)
            {
                return NotFound("Specialization not found.");
            }

            return NoContent();
        }
    }
}

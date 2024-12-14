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
            return Ok(await _service.GetAllSpecialization());
        }

        // 7.2.11: Adicionar nova especialização
        [HttpPost]
        public async Task<ActionResult<string>> AddSpecialization([FromBody] SpecializationCreateDTO dto)
        {
            if (string.IsNullOrEmpty(dto.Name))
            {
                return BadRequest("Specialization name is required.");
            }

            var specializationId = await _service.AddSpecialization(dto.Name);
            return CreatedAtAction(nameof(GetSpecializationById), new { id = specializationId }, specializationId);
        }

        // 7.2.12: Buscar especialização por ID
        [HttpGet("{id}")]
        public async Task<ActionResult<string>> GetSpecializationById(string id)
        {
            var specialization = await _service.GetSpecializationById(id);
            if (specialization == null)
            {
                return NotFound("Specialization not found.");
            }
            return Ok(specialization);
        }

        // 7.2.13: Atualizar uma especialização
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSpecialization(string id, [FromBody] SpecializationUpdateDTO dto)
        {
            if (string.IsNullOrEmpty(dto.Name))
            {
                return BadRequest("Specialization name is required.");
            }

            var updated = await _service.UpdateSpecialization(id, dto.Name);
            if (!updated)
            {
                return NotFound("Specialization not found.");
            }

            return NoContent();
        
        }
    }
}
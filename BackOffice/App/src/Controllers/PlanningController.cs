using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class PlanningController : ControllerBase
{
    private readonly PlanningService _planningService;

    public PlanningController(PlanningService planningService)
    {
        _planningService = planningService;
    }

    [HttpGet]
    public async Task<IActionResult> GetPlanning()
    {
        try
        {
            var planning = await _planningService.GetPlanningAsync();
            return Ok(planning);
        }
        catch (HttpRequestException ex)
        {
            return StatusCode(500, $"Erro ao buscar dados: {ex.Message}");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erro interno: {ex.Message}");
        }
    }
}

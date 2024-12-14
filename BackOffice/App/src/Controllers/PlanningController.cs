using Microsoft.AspNetCore.Mvc;
using Sempi5.Domain.OperationRequestEntity;
using Sempi5.Domain.RoomEntity;
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

    [HttpGet("obtain_better")]
    public async Task<IActionResult> ObtainBetter()
    {
        try
        {
            string day = "20251010";
            long roomId = 9;
            List<OperationRequest> operationRequests = new List<OperationRequest>();
            var planning = await _planningService.ScheduleOperations(day, roomId, operationRequests);
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

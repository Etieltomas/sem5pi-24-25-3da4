using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using Sempi5.Domain.OperationRequestEntity;
using Sempi5.Domain.RoomEntity;
using Sempi5.Domain.StaffEntity;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class PlanningController : ControllerBase
{
    private readonly PlanningService _planningService;
    private readonly IOperationRequestRepository _operationRequestRepository;

    public PlanningController(PlanningService planningService, IOperationRequestRepository operationRequestRepository)
    {
        _planningService = planningService;
        _operationRequestRepository = operationRequestRepository;
    }

    [HttpGet("obtain_better")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ObtainBetter()
    {
        try
        {
            string day = "20251010";
            long roomId = 9;
            
            var operationRequests = await _operationRequestRepository.GetAllOperationRequestsNotScheduled();

            var planning = await _planningService.ScheduleOperations(day, roomId, operationRequests);
            
            await UpdateScheduleRoom(planning.AgendaRoom, roomId, day);
            await UpdateScheduleDoctors(planning.AgendaDoctors, day);
            await CreateAppointments(day, planning.AgendaRoom, roomId, operationRequests);
            
            return Ok(new { message = "Operations schedule successfully", schedule = planning.AgendaRoom, doctors = planning.AgendaDoctors });
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

    private async Task CreateAppointments(string day, List<List<int>> agendaRoom, long roomID, List<OperationRequest> operationRequests)
    {
        await _planningService.CreateAppointments(day, agendaRoom, roomID, operationRequests);
    }

    private async Task UpdateScheduleDoctors(List<AgendaDoctor> agendaDoctors, string day)
    {
        await _planningService.UpdateScheduleDoctors(agendaDoctors, day);
    }

    private async Task UpdateScheduleRoom(List<List<int>> agendaRoom, long roomId, string day)
    {
        await _planningService.UpdateScheduleRoom(agendaRoom, roomId, day);
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

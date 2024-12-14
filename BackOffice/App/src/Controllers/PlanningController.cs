using Microsoft.AspNetCore.Mvc;
using Sempi5.Domain.OperationRequestEntity;
using Sempi5.Domain.RoomEntity;
using Sempi5.Domain.StaffEntity;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class PlanningController : ControllerBase
{
    private readonly PlanningService _planningService;
    private readonly IStaffRepository staffRep;
    private readonly IOperationTypeRepository operationTypeRep;
    

    public PlanningController(PlanningService planningService, IStaffRepository staffRep, IOperationTypeRepository operationTypeRep)
    {
        _planningService = planningService;
        this.staffRep = staffRep;
        this.operationTypeRep = operationTypeRep;
    }

    [HttpGet("obtain_better")]
    public async Task<IActionResult> ObtainBetter()
    {
        try
        {
            string day = "20251010";
            long roomId = 9;
            List<OperationRequest> operationRequests =
            [
                new OperationRequest {
                
                    Staff = await staffRep.GetStaffMemberByEmail("tomasandreleite@gmail.com"),
                    OperationType = await operationTypeRep.GetByIdAsync(new OperationTypeID(1)),
                    Priority = Priority.High,
                    Deadline = new Deadline(new DateTime(2025, 10, 10, 10, 0, 0)),
                    Status = Status.Pending,
                    Staffs = new List<Staff> { await staffRep.GetStaffMemberByEmail("tomasandreleite@gmail.com"),
                     await staffRep.GetStaffMemberByEmail("sblsimaolopes@gmail.com") }
                },
            ];
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

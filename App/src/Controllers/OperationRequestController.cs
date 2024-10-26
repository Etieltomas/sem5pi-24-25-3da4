using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class OperationRequestController : ControllerBase
{
    private readonly OperationRequestService _operationRequestService;

    public OperationRequestController(OperationRequestService operationRequestService)
    {
        _operationRequestService = operationRequestService;
    }

    [HttpPost]
    [Route("create")]
    public async Task<ActionResult<OperationRequestCreateDTO>> CreateOperationRequest([FromBody] OperationRequestCreateDTO dto)
    {
        try
        {
            var result = await _operationRequestService.CreateOperationRequest(dto);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut]
    [Route("update")]
    public async Task<ActionResult<OperationRequestUpdateDTO>> UpdateOperationRequest([FromBody] OperationRequestUpdateDTO dto)
    {
        try
        {
            var result = await _operationRequestService.UpdateOperationRequest(dto);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete]
    [Route("delete")]
    public async Task<ActionResult> DeleteOperationRequest([FromBody] OperationRequestUpdateDTO dto)
    {
        try
        {
            _operationRequestService.DeleteOperationRequest(dto.OperationRequestId, dto.StaffId);
            return Ok("Operation request successfully removed.");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
    [HttpGet]
    [Route("list")]
    public async Task<ActionResult> SearchOperationRequests([FromQuery] string? patientName, 
    [FromQuery] string? operationType, 
    [FromQuery] string? priority, 
    [FromQuery] string? status,
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 10)
    {
        try
        {
            var result =  await _operationRequestService.SearchOperationRequests(patientName, operationType, priority, status, page, pageSize);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

}

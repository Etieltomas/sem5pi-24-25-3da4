using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class OperationRequestCreateController : ControllerBase
{
    private readonly OperationRequestService _operationRequestService;

    public OperationRequestCreateController(OperationRequestService operationRequestService)
    {
        _operationRequestService = operationRequestService;
    }

    [HttpPost]
    [Route("create")]
    public IActionResult CreateOperationRequest([FromBody] OperationRequestCreateDTO dto)
    {
        try
        {
              var result = _operationRequestService.CreateOperationRequest(dto);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut]
    [Route("update")]
    public IActionResult UpdateOperationRequest([FromBody] OperationRequestUpdateDTO dto)
    {
        try
        {
            var result = _operationRequestService.UpdateOperationRequest(dto);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete]
    [Route("delete")]
    public IActionResult DeleteOperationRequest([FromQuery] int operationRequestId, [FromQuery] int staffId)
    {
        try
        {
            _operationRequestService.DeleteOperationRequest(operationRequestId, staffId);
            return Ok("Solicitação de operação removida com sucesso.");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
    [HttpGet]
    [Route("list")]
    public IActionResult SearchOperationRequests([FromQuery] string? patientName, [FromQuery] string? operationType, [FromQuery] string? priority, [FromQuery] string? status)
    {
        try
        {
            var result = _operationRequestService.SearchOperationRequests(patientName, operationType, priority, status);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

}

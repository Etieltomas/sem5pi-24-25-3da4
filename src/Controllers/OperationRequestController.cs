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

}

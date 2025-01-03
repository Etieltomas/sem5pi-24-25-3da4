using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class OperationRequestController : ControllerBase 
{
    private readonly OperationRequestService _operationRequestService;
    private readonly ILogger<OperationRequestController> _logger;

    public OperationRequestController(OperationRequestService operationRequestService, ILogger<OperationRequestController> logger)
    {
        _operationRequestService = operationRequestService;
        _logger = logger;
    }

/**
 * Handles POST request to create a new operation request.
 * @param dto OperationRequestCreateDTO - The data transfer object containing the details of the operation request to be created.
 * @return Task<ActionResult<OperationRequestCreateDTO>> - The result of the create operation, either a BadRequest if the operation fails or an Ok with the created operation request.
 * @author Ricardo Guimarães
 * @date 10/12/2024
 */
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


/**
 * Handles PUT request to update an existing operation request.
 * @param dto OperationRequestUpdateDTO - The data transfer object containing the updated details of the operation request.
 * @return Task<ActionResult<OperationRequestUpdateDTO>> - The result of the update operation, either a BadRequest if the operation fails or an Ok with the updated operation request.
 * @author Ricardo Guimarães
 * @date 10/12/2024
 */
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

/**
 * Handles DELETE request to remove an existing operation request.
 * @param dto OperationRequestUpdateDTO - The data transfer object containing the details of the operation request to be deleted.
 * @return Task<ActionResult> - The result of the delete operation, either a BadRequest if the operation fails or an Ok with a success message.
 * @author Ricardo Guimarães
 * @date 10/12/2024
 */
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
    
    /**
 * Handles GET request to retrieve operation requests by staff email.
 * @param email string - The email of the staff whose operation requests are to be retrieved.
 * @return Task<ActionResult<List<OperationRequestDto>>> - The result of the get operation, either a BadRequest if the operation fails or an Ok with the list of operation requests.
 * @author Ricardo Guimarães
 * @date 10/12/2024
 */
    [HttpGet]
    [Route("staff/{email}")]
    public async Task<ActionResult<List<OperationRequestDto>>> GetOperationRequestByStaff(string email)
    {

        try
        {
            var result =  await _operationRequestService.GetOperationRequestByStaff(email);
            
            _logger.LogInformation("Operation requests retrieved successfully.");

            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

/**
 * Handles GET request to search for operation requests based on various criteria.
 * @param patientName string? - The name of the patient (optional).
 * @param operationType string? - The type of the operation (optional).
 * @param priority string? - The priority of the operation request (optional).
 * @param status string? - The status of the operation request (optional).
 * @param page int - The page number for pagination (default is 1).
 * @param pageSize int - The number of items per page for pagination (default is 10).
 * @return Task<ActionResult> - The result of the search operation, either a BadRequest if the operation fails or an Ok with the search results.
 * @author Ricardo Guimarães
 * @date 10/12/2024
 */
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

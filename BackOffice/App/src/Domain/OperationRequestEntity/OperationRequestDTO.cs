/**
* This class is used to transfer data between the front-end and the back-end.
* @param Id int - The id of the operation request.
*     PatientName string - The name of the patient for whom the operation request is being made.
*     OperationType string - The type of the operation request.
*     Priority string - The priority of the operation request.
*     Deadline string - The deadline for the operation request.
*     StartDate string - The start date of the operation request.
*     EndDate string - The end date of the operation request.
*     Status string - The status of the operation request.
* @return OperationRequestDto - The data transfer object containing the details of the operation request.
* @author Ricardo Guimarães
* @date 10/12/2024
*/

public class OperationRequestDto
{
    public int? Id { get; set; }
    public string? PatientName { get; set; }
    public string? OperationType { get; set; }
    public string? Priority { get; set; }
    public string? Deadline { get; set; }
    public string? StartDate { get; set; }
    public string? EndDate { get; set; }
    public string? Status { get; set; }
}
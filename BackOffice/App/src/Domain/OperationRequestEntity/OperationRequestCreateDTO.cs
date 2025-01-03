
/**
* This class is used to create a new OperationRequest entity.
* @param StaffId string - The id of the staff member responsible for the operation request.
* @param PatientId string - The id of the patient for whom the operation request is being made.
* @param OperationTypeId string - The id of the operation type for the operation request.
* @param Priority string - The priority of the operation request.
* @param Deadline string - The deadline for the operation request.
* @return OperationRequestCreateDTO - The data transfer object containing the details of the new operation request.
* @author Ricardo Guimarães
* @date 10/12/2024
*/
public class OperationRequestCreateDTO
{
    public string StaffId { get; set; }
    public string PatientId { get; set; }
    public string OperationTypeId { get; set; }
    public string Priority { get; set; }
    public string Deadline { get; set; }
}

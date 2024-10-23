public class OperationRequestCreateDTO
{
    public int StaffId { get; set; }
    public int PatientId { get; set; }
    public int OperationTypeId { get; set; }
    public string Priority { get; set; }
    public DateTime Deadline { get; set; }
}

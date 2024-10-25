public class OperationRequestUpdateDTO
{
    public int OperationRequestId { get; set; }
    public int StaffId { get; set; }
    public string NewPriority { get; set; }
    public DateTime NewDeadline { get; set; }
}

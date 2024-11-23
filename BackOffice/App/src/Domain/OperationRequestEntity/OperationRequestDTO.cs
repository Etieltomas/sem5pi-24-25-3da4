public class OperationRequestDto
{
    public int Id { get; set; }
    public string PatientName { get; set; }
    public string OperationType { get; set; }
    public int Priority { get; set; }
    public DateTime Deadline { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Status { get; set; }
}
using Sempi5.Domain.StaffEntity;

public class PlanningDto
{
    public List<StaffDTO> AvailableStaffs { get; set; }
    public List<OperationRequestDto> ScheduledOperations { get; set; }
}
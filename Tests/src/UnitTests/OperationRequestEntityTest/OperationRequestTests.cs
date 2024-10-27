using Moq;
using Sempi5.Domain.OperationRequestEntity;
using Sempi5.Domain.PatientEntity;
using Sempi5.Domain.StaffEntity;
using Xunit;

public class OperationRequestTests
{
    [Fact]
    public void Should_Update_Priority_Successfully()
    {
        var operationRequest = new OperationRequest
        {
            Priority = Priority.Medium
        };

        operationRequest.UpdatePriority(Priority.High);

        Assert.Equal("High", operationRequest.Priority.Value);
    }

    [Fact]
    public void Should_Throw_Exception_When_Setting_Past_Deadline()
    {
        Assert.Throws<ArgumentException>(() => new Deadline(DateTime.Now.AddDays(-1)));
    }

    [Fact]
    public void Should_Set_Deadline_When_Future_Date()
    {
        var deadlineDate = DateTime.Now.AddDays(5);
        var deadline = new Deadline(deadlineDate);

        Assert.Equal(deadlineDate, deadline.Value);
    }
}

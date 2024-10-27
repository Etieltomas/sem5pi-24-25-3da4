using System;
using Xunit;
using Sempi5.Domain.OperationRequestEntity;

namespace Sempi5Test.DomainTests.OperationRequestEntityTest;

public class StatusTest
{
    [Fact]
    public void CanCreatePendingStatus()
    {
        var pendingStatus = Status.Pending;
        
        Assert.NotNull(pendingStatus);
        Assert.Equal("Pending", pendingStatus.Value);
    }

    [Fact]
    public void CanCreateCompletedStatus()
    {
        var completedStatus = Status.Completed;
        
        Assert.NotNull(completedStatus);
        Assert.Equal("Completed", completedStatus.Value);
    }

    [Fact]
    public void CanCreateCancelledStatus()
    {
        var cancelledStatus = Status.Cancelled;
        
        Assert.NotNull(cancelledStatus);
        Assert.Equal("Cancelled", cancelledStatus.Value);
    }

    [Theory]
    [InlineData("Pending")]
    [InlineData("Completed")]
    [InlineData("Cancelled")]
    public void CanCreateStatusFromString(string value)
    {
        var status = Status.FromString(value);
        
        Assert.NotNull(status);
        Assert.Equal(value, status.Value);
    }

    [Theory]
    [InlineData("InProgress")]
    [InlineData("None")]
    [InlineData("")]
    public void InvalidStatusFromStringThrowsException(string value)
    {
        Assert.Throws<ArgumentException>(() => Status.FromString(value));
    }

    [Fact]
    public void EqualStatusesShouldBeEqual()
    {
        var pendingStatus1 = Status.Pending;
        var pendingStatus2 = Status.Pending;

        Assert.Equal(pendingStatus1, pendingStatus2);
    }

    [Fact]
    public void DifferentStatusesShouldNotBeEqual()
    {
        var pendingStatus = Status.Pending;
        var completedStatus = Status.Completed;

        Assert.NotEqual(pendingStatus, completedStatus);
    }
}

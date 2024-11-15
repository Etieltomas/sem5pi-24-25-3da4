using System;
using Xunit;
using Sempi5.Domain.OperationRequestEntity;

namespace Sempi5Test.UnitTests.OperationRequestEntityTest;

public class PriorityTest
{
    [Fact]
    public void CanCreateHighPriority()
    {
        var highPriority = Priority.High;
        
        Assert.NotNull(highPriority);
        Assert.Equal("High", highPriority.Value);
    }

    [Fact]
    public void CanCreateMediumPriority()
    {
        var mediumPriority = Priority.Medium;
        
        Assert.NotNull(mediumPriority);
        Assert.Equal("Medium", mediumPriority.Value);
    }

    [Fact]
    public void CanCreateLowPriority()
    {
        var lowPriority = Priority.Low;
        
        Assert.NotNull(lowPriority);
        Assert.Equal("Low", lowPriority.Value);
    }

    [Theory]
    [InlineData("High")]
    [InlineData("Medium")]
    [InlineData("Low")]
    public void CanCreatePriorityFromString(string value)
    {
        var priority = Priority.FromString(value);
        
        Assert.NotNull(priority);
        Assert.Equal(value, priority.Value);
    }

    [Theory]
    [InlineData("Urgent")]
    [InlineData("None")]
    [InlineData("")]
    public void InvalidPriorityFromStringThrowsException(string value)
    {
        Assert.Throws<ArgumentException>(() => Priority.FromString(value));
    }

    [Fact]
    public void EqualPrioritiesShouldBeEqual()
    {
        var highPriority1 = Priority.High;
        var highPriority2 = Priority.High;

        Assert.Equal(highPriority1, highPriority2);
    }

    [Fact]
    public void DifferentPrioritiesShouldNotBeEqual()
    {
        var highPriority = Priority.High;
        var lowPriority = Priority.Low;

        Assert.NotEqual(highPriority, lowPriority);
    }
}

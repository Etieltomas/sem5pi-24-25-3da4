using System;
using Xunit;
using Sempi5.Domain.OperationRequestEntity;

namespace Sempi5.DomainTests.OperationRequestEntityTest;

public class DeadlineTest
{
    [Theory]
    [InlineData(5)]
    [InlineData(10)]
    public void CanCreateValidDeadline(int yearsFromNow)
    {
        var deadlineDate = DateTime.Now.AddYears(yearsFromNow);
        var deadline = new Deadline(deadlineDate);

        Assert.NotNull(deadline);
        Assert.Equal(deadlineDate, deadline.Value);
    }


    [Theory]
    [InlineData("2020-01-01")]
    [InlineData("2023-01-01")] 
    public void CreatingDeadlineInPastThrowsException(string date)
    {
        var deadlineDate = DateTime.Parse(date);
        
        Assert.Throws<ArgumentException>(() => new Deadline(deadlineDate));
    }

    [Fact]
    public void EqualDeadlinesShouldBeEqual()
    {
        var deadlineDate = DateTime.Parse("2025-12-31");
        var deadline1 = new Deadline(deadlineDate);
        var deadline2 = new Deadline(deadlineDate);

        Assert.Equal(deadline1, deadline2);
    }

    [Fact]
    public void DifferentDeadlinesShouldNotBeEqual()
    {
        var deadline1 = new Deadline(DateTime.Parse("2025-12-31"));
        var deadline2 = new Deadline(DateTime.Parse("2026-01-01"));

        Assert.NotEqual(deadline1, deadline2);
    }
}

using System;
using Xunit;
using Sempi5.Domain.PatientEntity;
using Sempi5.Domain.Shared;

namespace Sempi5Test.DomainTests.PatientEntityTest;

public class ConditionTest
{
    [Theory]
    [InlineData("Asthma")]
    [InlineData("Diabetes")]
    public void CanCreateValidCondition(string conditionValue)
    {
        var condition = new Condition(conditionValue);

        Assert.NotNull(condition);
        Assert.Equal(conditionValue, condition.ToString());
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void CreatingConditionWithInvalidValueThrowsException(string invalidValue)
    {
        Assert.Throws<BusinessRuleValidationException>(() => new Condition(invalidValue));
    }

    [Fact]
    public void ConditionsWithSameValueShouldBeEqual()
    {
        var condition1 = new Condition("Asthma");
        var condition2 = new Condition("Asthma");

        Assert.Equal(condition1.ToString(), condition2.ToString());
    }

    [Fact]
    public void ConditionsWithDifferentValuesShouldNotBeEqual()
    {
        var condition1 = new Condition("Asthma");
        var condition2 = new Condition("Diabetes");

        Assert.NotEqual(condition1.ToString(), condition2.ToString());
    }
}

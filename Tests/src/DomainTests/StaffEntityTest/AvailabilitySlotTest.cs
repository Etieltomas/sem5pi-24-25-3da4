using System;
using Xunit;
using Sempi5.Domain.Shared;
using Sempi5.Domain.StaffEntity;

namespace Sempi5Test.DomainTests.StaffEntityTest;

public class AvailabilitySlotTest
{
    [Theory]
    [InlineData("2024-10-21T09:00:00 - 2024-10-21T11:00:00")]
    [InlineData("2024-12-31T23:59:00 - 2025-01-01T01:00:00")]
    public void CanCreateValidAvailabilitySlot(string slotValue)
    {
        var availabilitySlot = new AvailabilitySlot(slotValue);

        Assert.NotNull(availabilitySlot);
        Assert.Equal(slotValue, availabilitySlot.ToString());
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void CreatingAvailabilitySlotWithInvalidValuesThrowsException(string invalidSlotValue)
    {
        Assert.Throws<BusinessRuleValidationException>(() => new AvailabilitySlot(invalidSlotValue));
    }

    [Theory]
    [InlineData("2024-10-21T11:00:00 - 2024-10-21T09:00:00")]
    [InlineData("2024-10-21T09:00:00 - 2024-10-21T09:00:00")]
    public void CreatingAvailabilitySlotWithInvalidTimesThrowsException(string invalidSlotValue)
    {
        Assert.Throws<BusinessRuleValidationException>(() => new AvailabilitySlot(invalidSlotValue));
    }

    [Theory]
    [InlineData("2024-10-21T09:00:00-2024-10-21T11:00:00")]
    [InlineData("2024-10-21T09:00:00 / 2024-10-21T11:00:00")]
    public void CreatingAvailabilitySlotWithInvalidFormatThrowsException(string invalidSlotValue)
    {
        Assert.Throws<BusinessRuleValidationException>(() => new AvailabilitySlot(invalidSlotValue));
    }
}

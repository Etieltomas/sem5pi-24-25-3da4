// AvailabilitySlotTest.cs
using System;
using Xunit;
using Sempi5.Domain.Shared;
using Sempi5.Domain.StaffEntity;

namespace Sempi5Test.UnitTests.StaffEntityTest
{
    public class AvailabilitySlotTest
    {
        [Theory]
        [InlineData(1)]
        [InlineData(2)] 
        public void CanCreateValidAvailabilitySlot(int hoursToAdd)
        {
            var startDateTime = DateTime.Now.AddHours(hoursToAdd);
            var endDateTime = startDateTime.AddHours(2);
            var availabilitySlot = new AvailabilitySlot(FormatDateTimeRange(startDateTime, endDateTime));

            Assert.NotNull(availabilitySlot);
            Assert.Equal(FormatDateTimeRange(startDateTime, endDateTime), availabilitySlot.ToString());
        }

        private string FormatDateTimeRange(DateTime startDateTime, DateTime endDateTime)
        {
            return $"{startDateTime:dd-MM-yyyyTHH:mm:ss} - {endDateTime:dd-MM-yyyyTHH:mm:ss}";
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
        [InlineData(1)] 
        [InlineData(0)] 
        public void CreatingAvailabilitySlotWithInvalidTimesThrowsException(int hoursToAdd)
        {
            var startDateTime = DateTime.Now.AddHours(hoursToAdd);
            var endDateTime = startDateTime.AddHours(-1); 
            Assert.Throws<BusinessRuleValidationException>(() => new AvailabilitySlot(FormatDateTimeRange(startDateTime, endDateTime)));
        }

        [Theory]
        [InlineData("21-10-2024T09:00:00-21-10-2024T11:00:00")]
        [InlineData("21-10-2024T09:00:00 / 21-10-2024T11:00:00")]
        public void CreatingAvailabilitySlotWithInvalidFormatThrowsException(string invalidSlotValue)
        {
            Assert.Throws<BusinessRuleValidationException>(() => new AvailabilitySlot(invalidSlotValue));
        }
    }
}
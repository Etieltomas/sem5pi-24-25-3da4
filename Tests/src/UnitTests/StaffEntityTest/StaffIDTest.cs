using Sempi5.Domain.StaffEntity;
using Xunit;

namespace Sempi5Test.UnitTests.StaffEntityTest
{
    public class StaffIDTests
    {

        [Fact]
        public void Constructor_WithValue_Should_SetValueCorrectly()
        {
            // Arrange
            var expectedValue = "S12345";

            // Act
            var staffId = new StaffID(expectedValue);

            // Assert
            Assert.Equal(expectedValue, staffId.AsString());
        }

        [Fact]
        public void AsString_Should_ReturnCorrectValue()
        {
            // Arrange
            var expectedValue = "S67890";
            var staffId = new StaffID(expectedValue);

            // Act
            var result = staffId.AsString();

            // Assert
            Assert.Equal(expectedValue, result);
        }
    }
}

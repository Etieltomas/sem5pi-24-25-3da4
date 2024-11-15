using Sempi5.Domain.UserEntity;
using Xunit;

namespace Sempi5Test.UnitTests.UserTest
{
    public class SystemUserIdTests
    {

        [Fact]
        public void Constructor_WithValue_Should_SetValueCorrectly()
        {
            // Arrange
            long expectedValue = 12345;

            // Act
            var userId = new SystemUserId(expectedValue);

            // Assert
            Assert.Equal(expectedValue, userId.AsLong());
            Assert.Equal(expectedValue.ToString(), userId.AsString());
        }

        [Fact]
        public void AsString_Should_ReturnCorrectValue()
        {
            // Arrange
            long expectedValue = 67890;
            var userId = new SystemUserId(expectedValue);

            // Act
            var result = userId.AsString();

            // Assert
            Assert.Equal(expectedValue.ToString(), result);
        }

        [Fact]
        public void AsLong_Should_ReturnCorrectLongValue()
        {
            // Arrange
            long expectedValue = 54321;
            var userId = new SystemUserId(expectedValue);

            // Act
            var result = userId.AsLong();

            // Assert
            Assert.Equal(expectedValue, result);
        }
    }
}

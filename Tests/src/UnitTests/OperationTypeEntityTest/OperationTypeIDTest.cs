using Sempi5.Domain.OperationRequestEntity;
using Xunit;

namespace Sempi5Test.UnitTests.OperationTypeEntityTest
{
    public class OperationTypeIDTest
    {
        [Fact]
        public void Constructor_Should_SetValueCorrectly()
        {
            // Arrange
            long expectedValue = 12345;

            // Act
            var operationTypeId = new OperationTypeID(expectedValue);

            // Assert
            Assert.Equal(expectedValue, operationTypeId.AsLong());
        }

        [Fact]
        public void AsString_Should_ReturnStringRepresentationOfValue()
        {
            // Arrange
            long value = 67890;
            var operationTypeId = new OperationTypeID(value);

            // Act
            var result = operationTypeId.AsString();

            // Assert
            Assert.Equal(value.ToString(), result);
        }

        [Fact]
        public void AsLong_Should_ReturnLongValue()
        {
            // Arrange
            long value = 54321;
            var operationTypeId = new OperationTypeID(value);

            // Act
            var result = operationTypeId.AsLong();

            // Assert
            Assert.Equal(value, result);
        }
    }
}

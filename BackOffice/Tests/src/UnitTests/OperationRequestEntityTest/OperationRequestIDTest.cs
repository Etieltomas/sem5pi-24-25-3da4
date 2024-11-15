using Sempi5.Domain.OperationRequestEntity;
using Xunit;

namespace Sempi5Test.UnitTests.OperationRequestEntityTest
{
    public class OperationRequestIDTest
    {
        [Fact]
        public void Constructor_Should_SetValueCorrectly()
        {
            // Arrange
            var expectedValue = "12345";

            // Act
            var operationRequestId = new OperationRequestID(expectedValue);

            // Assert
            Assert.Equal(expectedValue, operationRequestId.AsString());
        }

        [Fact]
        public void AsString_Should_ReturnCorrectValue()
        {
            // Arrange
            var expectedValue = "67890";
            var operationRequestId = new OperationRequestID(expectedValue);

            // Act
            var result = operationRequestId.AsString();

            // Assert
            Assert.Equal(expectedValue, result);
        }

        [Fact]
        public void CreateFromString_Should_ReturnStringAsObject()
        {
            // Arrange
            var textValue = "54321";
            var operationRequestId = new OperationRequestID(textValue);

            // Act
            var result = operationRequestId.AsString();

            // Assert
            Assert.IsType<string>(result);
            Assert.Equal(textValue, result);
        }
    }
}

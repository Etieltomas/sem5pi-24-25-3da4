using Sempi5.Domain.PatientEntity;
using Xunit;

namespace Sempi5Test.UnitTests.PatientEntityTest
{
    public class PatientIDTest
    {
        [Fact]
        public void Constructor_Should_SetValueCorrectly()
        {
            // Arrange
            var expectedValue = "P12345";

            // Act
            var patientId = new PatientID(expectedValue);

            // Assert
            Assert.Equal(expectedValue, patientId.AsString());
        }

        [Fact]
        public void AsString_Should_ReturnCorrectValue()
        {
            // Arrange
            var expectedValue = "P67890";
            var patientId = new PatientID(expectedValue);

            // Act
            var result = patientId.AsString();

            // Assert
            Assert.Equal(expectedValue, result);
        }
    }
}

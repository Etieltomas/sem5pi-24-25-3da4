using Moq;
using Sempi5.Domain.OperationRequestEntity;
using Sempi5.Domain.SpecializationEntity;
using Xunit;

namespace Sempi5Test.UnitTests.OperationTypeEntityTest
{
    public class OperationTypeTest
    {
        [Fact]
        public void Constructor_Should_SetPropertiesCorrectly()
        {
            // Arrange
            var mockSpecializationID = new Mock<SpecializationID>("Surgery");
            var mockSpecialization = new Mock<Specialization>(mockSpecializationID.Object);
            var operationTypeName = "Team Surgery";

            // Act
            var operationType = new OperationType
            {
                Name = operationTypeName,
                Anesthesia_Duration = 2,
                Surgery_Duration = 4,
                Cleaning_Duration = 1,
                Specialization = mockSpecialization.Object
            };

            // Assert
            Assert.Equal(operationTypeName, operationType.Name);
            Assert.Equal(mockSpecialization.Object, operationType.Specialization);
        }

        [Fact]
        public void SetName_Should_UpdateName()
        {
            // Arrange
            var operationType = new OperationType();
            var newName = "New Surgery Type";

            // Act
            operationType.Name = newName;

            // Assert
            Assert.Equal(newName, operationType.Name);
        }

        [Fact]
        public void SetSpecialization_Should_UpdateSpecialization()
        {
            // Arrange
            var mockSpecializationID = new Mock<SpecializationID>("Surgery");
            var mockSpecialization = new Mock<Specialization>(mockSpecializationID.Object);
            var operationType = new OperationType();

            // Act
            operationType.Specialization = mockSpecialization.Object;

            // Assert
            Assert.Equal(mockSpecialization.Object, operationType.Specialization);
        }
    }
}

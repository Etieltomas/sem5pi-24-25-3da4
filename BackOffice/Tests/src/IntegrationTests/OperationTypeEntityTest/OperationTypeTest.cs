using Sempi5.Domain.OperationRequestEntity;
using Sempi5.Domain.SpecializationEntity;
using Xunit;

namespace Sempi5Test.IntegrationTests.OperationTypeEntityTest
{
    public class OperationTypeTestsWithoutMock
    {
        [Fact]
        public void Constructor_Should_SetPropertiesCorrectly()
        {
            // Arrange
            var specialization = new Specialization ( new SpecializationID("Cardiology" ));
            var operationTypeName = "Heart Surgery";

            // Act
            var operationType = new OperationType
            {
                Name = operationTypeName,
                Anesthesia_Duration = 2,
                Surgery_Duration = 4,
                Cleaning_Duration = 1,
                Specialization = specialization
            };

            // Assert
            Assert.Equal(operationTypeName, operationType.Name);
            Assert.Equal(specialization, operationType.Specialization);
        }

        [Fact]
        public void SetName_Should_UpdateName()
        {
            // Arrange
            var operationType = new OperationType();
            var newName = "Updated Surgery Type";

            // Act
            operationType.Name = newName;

            // Assert
            Assert.Equal(newName, operationType.Name);
        }

        [Fact]
        public void SetSpecialization_Should_UpdateSpecialization()
        {
            // Arrange
            var specialization = new Specialization ( new SpecializationID("Neurology" ));
            var operationType = new OperationType();

            // Act
            operationType.Specialization = specialization;

            // Assert
            Assert.Equal(specialization, operationType.Specialization);
        }
    }
}

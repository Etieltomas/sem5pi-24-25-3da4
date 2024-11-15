using Moq;
using Sempi5.Domain.SpecializationEntity;
using Xunit;

namespace Sempi5Test.IntegrationTests.SpecializationEntityTest
{
    public class SpecializationTests
    {
        [Fact]
        public void Constructor_Should_SetIdCorrectly()
        {
            // Arrange
            var specializationId = new SpecializationID("Cardiology"); 
            var specialization = new Specialization(specializationId);

            // Act & Assert
            Assert.Equal(specializationId, specialization.Id);
        }
        
    }
}

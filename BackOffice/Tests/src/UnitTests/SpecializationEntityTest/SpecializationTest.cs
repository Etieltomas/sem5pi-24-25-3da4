using Moq;
using Sempi5.Domain.SpecializationEntity;
using Xunit;

namespace Sempi5Test.UnitTests.SpecializationEntityTest
{
    public class SpecializationTests
    {
        [Fact]
        public void Constructor_Should_SetIdCorrectly()
        {
            // Arrange
            var specializationIdMock = new Mock<SpecializationID>("Cardiology"); 
            var specialization = new Specialization(specializationIdMock.Object);

            // Act & Assert
            Assert.Equal(specializationIdMock.Object, specialization.Id);
        }
        
    }
}

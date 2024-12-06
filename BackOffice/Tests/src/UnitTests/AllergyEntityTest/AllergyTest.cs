using Moq;
using Sempi5.Domain.AllergyEntity;
using Sempi5.Domain.Shared;
using Xunit;

namespace Sempi5Test.UnitTests.AllergyEntityTest
{
    public class AllergyTests
    {
        [Fact]
        public void Allergy_Should_Initialize_With_Mocked_Dependencies()
        {
            // Arrange
            var mockCode = new Mock<Code>(123);
            mockCode.Setup(c => c.ToInt()).Returns(123);

            var mockName = new Mock<Name>("Peanut");
            mockName.Setup(n => n.ToString()).Returns("Peanut");

            // Act
            var allergy = new Allergy
            {
                Code = mockCode.Object,
                Name = mockName.Object
            };

            // Assert
            Assert.NotNull(allergy.Code);
            Assert.NotNull(allergy.Name);
            Assert.Equal(123, allergy.Code.ToInt());
            Assert.Equal("Peanut", allergy.Name.ToString());
        }
    }
}

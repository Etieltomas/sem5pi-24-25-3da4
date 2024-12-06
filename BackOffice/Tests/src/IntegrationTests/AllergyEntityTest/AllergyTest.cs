using Sempi5.Domain.AllergyEntity;
using Sempi5.Domain.Shared;
using Xunit;

namespace Sempi5Test.IntegrationTests.AllergyEntityTest
{
    public class AllergyTests
    {
        [Fact]
        public void Allergy_Should_Initialize_Properties()
        {
            // Arrange
            var code = new Code(123); 
            var name = new Name ("Peanut"); 

            // Act
            var allergy = new Allergy
            {
                Code = code,
                Name = name
            };

            // Assert
            Assert.NotNull(allergy.Code);
            Assert.NotNull(allergy.Name);
            Assert.Equal(123, allergy.Code.ToInt());
            Assert.Equal("Peanut", allergy.Name.ToString());
        }
    }
}

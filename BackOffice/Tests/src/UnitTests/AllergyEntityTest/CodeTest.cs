using Sempi5.Domain.AllergyEntity;
using Xunit;

namespace Sempi5Test.UnitTests.AllergyEntityTest
{
    public class CodeTest
    {
        [Fact]
        public void Constructor_Should_Initialize_Value_Correctly()
        {
            // Arrange
            int expectedValue = 123;

            // Act
            var code = new Code(expectedValue);

            // Assert
            Assert.Equal(expectedValue, code.ToInt());
        }
    }
}

using Sempi5.Domain.TokenEntity;
using System;
using Xunit;

namespace Sempi5Test.UnitTests.TokenTest
{
    public class TokenIDTests
    {
        [Fact]
        public void Constructor_WithGuid_Should_SetValueCorrectly()
        {
            // Arrange
            var expectedGuid = Guid.NewGuid();

            // Act
            var tokenId = new TokenID(expectedGuid);

            // Assert
            Assert.Equal(expectedGuid, tokenId.AsGuid());
        }

        [Fact]
        public void Constructor_WithString_Should_SetValueCorrectly()
        {
            // Arrange
            var expectedGuid = Guid.NewGuid();
            var expectedString = expectedGuid.ToString();

            // Act
            var tokenId = new TokenID(expectedString);

            // Assert
            Assert.Equal(expectedGuid, tokenId.AsGuid());
        }

        [Fact]
        public void AsString_Should_ReturnCorrectValue()
        {
            // Arrange
            var expectedGuid = Guid.NewGuid();
            var tokenId = new TokenID(expectedGuid);

            // Act
            var result = tokenId.AsString();

            // Assert
            Assert.Equal(expectedGuid.ToString(), result);
        }

        [Fact]
        public void AsGuid_Should_ReturnCorrectGuidValue()
        {
            // Arrange
            var expectedGuid = Guid.NewGuid();
            var tokenId = new TokenID(expectedGuid);

            // Act
            var result = tokenId.AsGuid();

            // Assert
            Assert.Equal(expectedGuid, result);
        }

    }
}

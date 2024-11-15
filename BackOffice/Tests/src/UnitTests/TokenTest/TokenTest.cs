using System;
using Xunit;
using Moq;
using Sempi5.Domain.Shared;
using Sempi5.Domain.TokenEntity;

namespace Sempi5Test.UnitTests.TokenTest;

public class TokenTest
{
    [Fact]
    public void CanCreateValidToken()
    {
        // Arrange
        var emailMock = new Mock<Email>("test@example.com");
        emailMock.Setup(e => e.ToString()).Returns("test@example.com");

        var expirationDate = DateTime.UtcNow.AddDays(1);

        var token = new Token
        {
            Email = emailMock.Object,
            ExpirationDate = expirationDate,
            IsUsed = false
        };

        // Assert
        Assert.NotNull(token);
        Assert.Equal(emailMock.Object.ToString(), token.Email.ToString());
        Assert.Equal(expirationDate, token.ExpirationDate);
        Assert.False(token.IsUsed);
    }

    [Fact]
    public void SettingIsUsedToTrue()
    {
        // Arrange
        var emailMock = new Mock<Email>("test@example.com");
        
        var token = new Token
        {
            Email = emailMock.Object,
            ExpirationDate = DateTime.UtcNow.AddDays(1),
            IsUsed = false
        };

        // Act
        token.IsUsed = true;

        // Assert
        Assert.True(token.IsUsed);
    }

    [Fact]
    public void TokenExpirationDateCannotBeInThePast()
    {
        // Arrange
        var emailMock = new Mock<Email>("test@example.com");

        // Act & Assert
        Assert.Throws<BusinessRuleValidationException>(() => new Token
        {
            Email = emailMock.Object,
            ExpirationDate = DateTime.UtcNow.AddDays(-1),
            IsUsed = false
        });
    }

    [Fact]
    public void TokensWithSameValuesShouldBeEqual()
    {
        // Arrange
        var emailMock = new Mock<Email>("test@example.com");
        emailMock.Setup(e => e.ToString()).Returns("test@example.com");

        var expirationDate = DateTime.UtcNow.AddDays(1);

        var token1 = new Token
        {
            Email = emailMock.Object,
            ExpirationDate = expirationDate,
            IsUsed = false
        };

        var token2 = new Token
        {
            Email = emailMock.Object,
            ExpirationDate = expirationDate,
            IsUsed = false
        };

        // Assert
        Assert.Equal(token1.Email.ToString(), token2.Email.ToString());
        Assert.Equal(token1.ExpirationDate, token2.ExpirationDate);
        Assert.Equal(token1.IsUsed, token2.IsUsed);
    }
}

using System;
using Xunit;
using Sempi5.Domain.Shared;
using Sempi5.Domain.TokenEntity;

public class TokenTest
{
    [Fact]
    public void CanCreateValidToken()
    {
        var email = new Email("test@example.com");
        var expirationDate = DateTime.UtcNow.AddDays(1);
        var token = new Token
        {
            Email = email,
            ExpirationDate = expirationDate,
            IsUsed = false
        };

        Assert.NotNull(token);
        Assert.Equal(email, token.Email);
        Assert.Equal(expirationDate, token.ExpirationDate);
        Assert.False(token.IsUsed);
    }

    [Fact]
    public void SettingIsUsedToTrue()
    {
        var token = new Token
        {
            Email = new Email("test@example.com"),
            ExpirationDate = DateTime.UtcNow.AddDays(1),
            IsUsed = false
        };

        token.IsUsed = true;

        Assert.True(token.IsUsed);
    }

    [Fact]
    public void TokenExpirationDateCannotBeInThePast()
    {
        var email = new Email("test@example.com");

        Assert.Throws<BusinessRuleValidationException>(() => new Token
        {
            Email = email,
            ExpirationDate = DateTime.UtcNow.AddDays(-1),
            IsUsed = false
        });
    }

    [Fact]
    public void TokensWithSameValuesShouldBeEqual()
    {
        var email = new Email("test@example.com");
        var expirationDate = DateTime.UtcNow.AddDays(1);
        
        var token1 = new Token
        {
            Email = email,
            ExpirationDate = expirationDate,
            IsUsed = false
        };

        var token2 = new Token
        {
            Email = email,
            ExpirationDate = expirationDate,
            IsUsed = false
        };

        Assert.Equal(token1.Email, token2.Email);
        Assert.Equal(token1.ExpirationDate, token2.ExpirationDate);
        Assert.Equal(token1.IsUsed, token2.IsUsed);
    }
}

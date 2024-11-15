using System;
using Xunit;
using Sempi5.Domain.UserEntity;
using Sempi5.Domain.Shared;

namespace Sempi5Test.IntegrationTests.UserTest;

public class SystemUserTest
{
    [Fact]
    public void CanInitializeSystemUser()
    {
        var systemUser = new SystemUser
        {
            Username = "franciscoaguiar",
            Role = "Admin",
            Email = new Email("franciscoaguiar@example.com"),
            Active = true,
            MarketingConsent = true
        };

        Assert.NotNull(systemUser);
        Assert.Equal("franciscoaguiar", systemUser.Username);
        Assert.Equal("Admin", systemUser.Role);
        Assert.Equal("franciscoaguiar@example.com", systemUser.Email.ToString());
        Assert.True(systemUser.Active);
        Assert.True(systemUser.MarketingConsent);
    }

    [Fact]
    public void CanUpdateSystemUserProperties()
    {
        var systemUser = new SystemUser
        {
            Username = "initialUser",
            Role = "User",
            Email = new Email("initial@example.com"),
            Active = true,
            MarketingConsent = false
        };

        systemUser.Username = "updatedUser";
        systemUser.Role = "Admin";
        systemUser.Email = new Email("updated@example.com");
        systemUser.Active = false;
        systemUser.MarketingConsent = true;

        Assert.Equal("updatedUser", systemUser.Username);
        Assert.Equal("Admin", systemUser.Role);
        Assert.Equal("updated@example.com", systemUser.Email.ToString());
        Assert.False(systemUser.Active);
        Assert.True(systemUser.MarketingConsent);
    }

    [Fact]
    public void SystemUser_DefaultMarketingConsent_ShouldBeFalse()
    {
        var systemUser = new SystemUser
        {
            Username = "userWithoutConsent",
            Role = "User",
            Email = new Email("userWithoutConsent@example.com"),
            Active = true
        };

        Assert.False(systemUser.MarketingConsent);
    }

    [Fact]
    public void SystemUser_CanToggleActiveStatus()
    {
        var systemUser = new SystemUser
        {
            Username = "toggleUser",
            Role = "User",
            Email = new Email("toggleUser@example.com"),
            Active = true
        };

        systemUser.Active = !systemUser.Active;
        Assert.False(systemUser.Active);

        systemUser.Active = !systemUser.Active;
        Assert.True(systemUser.Active);
    }

    [Fact]
    public void SystemUsersWithSameValues_ShouldBeEqual()
    {
        var systemUser1 = new SystemUser
        {
            Username = "franciscoaguiar",
            Role = "Admin",
            Email = new Email("franciscoaguiar@example.com"),
            Active = true,
            MarketingConsent = true
        };

        var systemUser2 = new SystemUser
        {
            Username = "franciscoaguiar",
            Role = "Admin",
            Email = new Email("franciscoaguiar@example.com"),
            Active = true,
            MarketingConsent = true
        };

        Assert.Equal(systemUser1.Username, systemUser2.Username);
        Assert.Equal(systemUser1.Role, systemUser2.Role);
        Assert.Equal(systemUser1.Email.ToString(), systemUser2.Email.ToString());
        Assert.Equal(systemUser1.Active, systemUser2.Active);
        Assert.Equal(systemUser1.MarketingConsent, systemUser2.MarketingConsent);
    }

    [Fact]
    public void SystemUsersWithDifferentValues_ShouldNotBeEqual()
    {
        var systemUser1 = new SystemUser
        {
            Username = "franciscoaguiar",
            Role = "Admin",
            Email = new Email("franciscoaguiar@example.com"),
            Active = true,
            MarketingConsent = true
        };

        var systemUser2 = new SystemUser
        {
            Username = "saraaguiar",
            Role = "User",
            Email = new Email("saraaguiar@example.com"),
            Active = false,
            MarketingConsent = false
        };

        Assert.NotEqual(systemUser1.Username, systemUser2.Username);
        Assert.NotEqual(systemUser1.Role, systemUser2.Role);
        Assert.NotEqual(systemUser1.Email.ToString(), systemUser2.Email.ToString());
        Assert.NotEqual(systemUser1.Active, systemUser2.Active);
        Assert.NotEqual(systemUser1.MarketingConsent, systemUser2.MarketingConsent);
    }
}

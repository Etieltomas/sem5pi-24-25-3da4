using System;
using Xunit;
using Sempi5.Domain.UserEntity;
using Sempi5.Domain.Shared;
using Moq;

namespace Sempi5Test.DomainTests.UserTest;

public class SystemUserTest
{
    [Fact]
    public void CanInitializeSystemUser()
    {
        var emailMock = new Mock<Email>("franciscoaguiar@example.com");
        emailMock.Setup(e => e.ToString()).Returns("franciscoaguiar@example.com");

        var systemUser = new SystemUser
        {
            Username = "franciscoaguiar",
            Role = "Admin",
            Email = emailMock.Object,
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
        var emailMock = new Mock<Email>("initial@example.com");
        emailMock.Setup(e => e.ToString()).Returns("initial@example.com");   

        var systemUser = new SystemUser
        {
            Username = "initialUser",
            Role = "User",
            Email = emailMock.Object,
            Active = true,
            MarketingConsent = false
        };

        var emailMock1 = new Mock<Email>("update@example.com");
        emailMock1.Setup(e => e.ToString()).Returns("update@example.com");
        
        systemUser.Username = "updatedUser";
        systemUser.Role = "Admin";
        systemUser.Email = emailMock1.Object;
        systemUser.Active = false;
        systemUser.MarketingConsent = true;

        Assert.Equal("updatedUser", systemUser.Username);
        Assert.Equal("Admin", systemUser.Role);
        Assert.Equal("update@example.com", systemUser.Email.ToString());
        Assert.False(systemUser.Active);
        Assert.True(systemUser.MarketingConsent);
    }

    [Fact]
    public void SystemUser_DefaultMarketingConsent_ShouldBeFalse()
    {
        var emailMock = new Mock<Email>("userWithoutConsent@example.com");
        
        var systemUser = new SystemUser
        {
            Username = "userWithoutConsent",
            Role = "User",
            Email = emailMock.Object,
            Active = true
        };

        Assert.False(systemUser.MarketingConsent);
    }

    [Fact]
    public void SystemUser_CanToggleActiveStatus()
    {
        var emailMock = new Mock<Email>("toggleUser@example.com");

        var systemUser = new SystemUser
        {
            Username = "toggleUser",
            Role = "User",
            Email = emailMock.Object,
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
        var emailMock = new Mock<Email>("franciscoaguiar@example.com");
        emailMock.Setup(e => e.ToString()).Returns("franciscoaguiar@example.com");  

        var systemUser1 = new SystemUser
        {
            Username = "franciscoaguiar",
            Role = "Admin",
            Email = emailMock.Object,
            Active = true,
            MarketingConsent = true
        };

        var systemUser2 = new SystemUser
        {
            Username = "franciscoaguiar",
            Role = "Admin",
            Email = emailMock.Object,
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
        var emailMock = new Mock<Email>("franciscoaguiar@example.com");
        emailMock.Setup(e => e.ToString()).Returns("franciscoaguiar@example.com");

        var systemUser1 = new SystemUser
        {
            Username = "franciscoaguiar",
            Role = "Admin",
            Email = emailMock.Object,
            Active = true,
            MarketingConsent = true
        };

        var emailMock2 = new Mock<Email>("saraaguiar@example.com");
        emailMock2.Setup(e => e.ToString()).Returns("saraaguiar@example.com");

        var systemUser2 = new SystemUser
        {
            Username = "saraaguiar",
            Role = "User",
            Email = emailMock2.Object,
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

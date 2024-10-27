using System;
using Xunit;
using Sempi5.Domain.Shared;
using System.Text.RegularExpressions;

namespace Sempi5Test.UnitTests.SharedTest;

public class EmailTest
{
    [Theory]
    [InlineData("example@example.com")]
    [InlineData("user.name+tag+sorting@example.com")]
    [InlineData("x@example.com")]
    public void CanCreateValidEmail(string emailValue)
    {
        var email = new Email(emailValue);

        Assert.NotNull(email);
        Assert.Equal(emailValue, email.ToString());
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("plainaddress")]
    [InlineData("@missingusername.com")]
    [InlineData("missingatsign.com")]
    public void CreatingEmailWithInvalidValuesThrowsException(string invalidEmailValue)
    {
        Assert.Throws<BusinessRuleValidationException>(() => new Email(invalidEmailValue));
    }

    [Fact]
    public void EmailsWithSameValueShouldBeEqual()
    {
        var email1 = new Email("example@example.com");
        var email2 = new Email("example@example.com");

        Assert.Equal(email1.ToString(), email2.ToString());
    }

    [Fact]
    public void EmailsWithDifferentValuesShouldNotBeEqual()
    {
        var email1 = new Email("example1@example.com");
        var email2 = new Email("example2@example.com");

        Assert.NotEqual(email1.ToString(), email2.ToString());
    }
}

using System;
using Xunit;
using Sempi5.Domain.Shared;

namespace Sempi5Test.UnitTests.SharedTest;
public class PhoneTest
{
    [Theory]
    [InlineData("123-456-7890")]
    [InlineData("+1-800-555-5555")]
    [InlineData("(123) 456-7890")]
    public void CanCreateValidPhone(string phoneValue)
    {
        var phone = new Phone(phoneValue);

        Assert.NotNull(phone);
        Assert.Equal(phoneValue, phone.ToString());
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void CreatingPhoneWithInvalidValuesThrowsException(string invalidPhoneValue)
    {
        Assert.Throws<BusinessRuleValidationException>(() => new Phone(invalidPhoneValue));
    }

    [Fact]
    public void PhonesWithSameValueShouldBeEqual()
    {
        var phone1 = new Phone("123-456-7890");
        var phone2 = new Phone("123-456-7890");

        Assert.Equal(phone1.ToString(), phone2.ToString());
    }

    [Fact]
    public void PhonesWithDifferentValuesShouldNotBeEqual()
    {
        var phone1 = new Phone("123-456-7890");
        var phone2 = new Phone("098-765-4321");

        Assert.NotEqual(phone1.ToString(), phone2.ToString());
    }
}

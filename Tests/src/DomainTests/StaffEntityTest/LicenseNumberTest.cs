using System;
using Xunit;
using Sempi5.Domain.Shared;
using Sempi5.Domain.StaffEntity;

namespace Sempi5.DomainTests.StaffEntityTest;

public class LicenseNumberTest
{
    [Theory]
    [InlineData("ABC123")]
    [InlineData("1234567890")]
    [InlineData("XYZ-987654")]
    public void CanCreateValidLicenseNumber(string licenseValue)
    {
        var licenseNumber = new LicenseNumber(licenseValue);

        Assert.NotNull(licenseNumber);
        Assert.Equal(licenseValue, licenseNumber.ToString());
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void CreatingLicenseNumberWithInvalidValuesThrowsException(string invalidLicenseValue)
    {
        Assert.Throws<BusinessRuleValidationException>(() => new LicenseNumber(invalidLicenseValue));
    }

    [Fact]
    public void LicenseNumbersWithSameValueShouldBeEqual()
    {
        var licenseNumber1 = new LicenseNumber("ABC123");
        var licenseNumber2 = new LicenseNumber("ABC123");

        Assert.Equal(licenseNumber1.ToString(), licenseNumber2.ToString());
    }

    [Fact]
    public void LicenseNumbersWithDifferentValuesShouldNotBeEqual()
    {
        var licenseNumber1 = new LicenseNumber("ABC123");
        var licenseNumber2 = new LicenseNumber("XYZ987");

        Assert.NotEqual(licenseNumber1.ToString(), licenseNumber2.ToString());
    }
}

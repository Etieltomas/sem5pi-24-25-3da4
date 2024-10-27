using System;
using Xunit;
using Sempi5.Domain.Shared;

namespace Sempi5Test.UnitTests.SharedTest;
public class AddressTest
{
    [Theory]
    [InlineData("123 Main St", "Springfield", "IL")]
    [InlineData("456 Elm St", "Gotham", "NY")]
    public void CanCreateValidAddress(string street, string city, string state)
    {
        var address = new Address(street, city, state);

        Assert.NotNull(address);
        Assert.Equal(street, address.ToString().Split(',')[0].Trim());
        Assert.Equal(city, address.ToString().Split(',')[1].Trim());
        Assert.Equal(state, address.ToString().Split(',')[2].Trim());
    }

    [Theory]
    [InlineData("", "Springfield", "IL")]
    [InlineData("123 Main St", "", "IL")]
    [InlineData("123 Main St", "Springfield", "")]
    [InlineData(null, "Springfield", "IL")]
    [InlineData("123 Main St", null, "IL")]
    [InlineData("123 Main St", "Springfield", null)]
    public void CreatingAddressWithInvalidValuesThrowsException(string street, string city, string state)
    {
        Assert.Throws<BusinessRuleValidationException>(() => new Address(street, city, state));
    }

    [Fact]
    public void AddressesWithSameValuesShouldBeEqual()
    {
        var address1 = new Address("123 Main St", "Springfield", "IL");
        var address2 = new Address("123 Main St", "Springfield", "IL");

        Assert.Equal(address1.ToString(), address2.ToString());
    }

    [Fact]
    public void AddressesWithDifferentValuesShouldNotBeEqual()
    {
        var address1 = new Address("123 Main St", "Springfield", "IL");
        var address2 = new Address("456 Elm St", "Gotham", "NY");

        Assert.NotEqual(address1.ToString(), address2.ToString());
    }
}

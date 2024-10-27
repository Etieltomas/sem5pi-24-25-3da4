using System;
using Xunit;
using Sempi5.Domain.Shared;

namespace Sempi5Test.DomainTests.SharedTest;

public class NameTests
{
    [Theory]
    [InlineData("John Doe")]
    [InlineData("Jane Smith")]
    public void CanCreateValidName(string nameValue)
    {
        var name = new Name(nameValue);

        Assert.NotNull(name);
        Assert.Equal(nameValue, name.ToString());
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData(" ")]
    public void CreatingNameWithInvalidValuesThrowsException(string invalidNameValue)
    {
        Assert.Throws<BusinessRuleValidationException>(() => new Name(invalidNameValue));
    }

    [Theory]
    [InlineData("John Doe", "John")]
    [InlineData("Jane Smith", "Jane")]
    [InlineData("Mononym", "Mononym")]
    public void FirstName_ReturnsCorrectFirstName(string nameValue, string expectedFirstName)
    {
        var name = new Name(nameValue);

        Assert.Equal(expectedFirstName, name.FirstName());
    }

    [Theory]
    [InlineData("John Doe", "Doe")]
    [InlineData("Jane Smith", "Smith")]
    [InlineData("Mononym", "Mononym")]
    public void LastName_ReturnsCorrectLastName(string nameValue, string expectedLastName)
    {
        var name = new Name(nameValue);

        Assert.Equal(expectedLastName, name.LastName());
    }

    [Fact]
    public void NamesWithSameValueShouldBeEqual()
    {
        var name1 = new Name("John Doe");
        var name2 = new Name("John Doe");

        Assert.Equal(name1.ToString(), name2.ToString());
    }

    [Fact]
    public void NamesWithDifferentValuesShouldNotBeEqual()
    {
        var name1 = new Name("John Doe");
        var name2 = new Name("Jane Doe");

        Assert.NotEqual(name1.ToString(), name2.ToString());
    }
}

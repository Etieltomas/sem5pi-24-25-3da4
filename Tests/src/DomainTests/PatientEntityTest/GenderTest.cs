using System;
using Xunit;
using Sempi5.Domain.PatientEntity;
using Sempi5.Domain.Shared;

public class GenderTest
{
    [Theory]
    [InlineData("male", Gender.Male)]
    [InlineData("female", Gender.Female)]
    [InlineData("nonBinary", Gender.Other)]
    [InlineData("Male", Gender.Male)] 
    [InlineData("Female", Gender.Female)] 
    [InlineData("NonBinary", Gender.Other)] 
    public void FromString_ValidGenderStrings_ReturnsExpectedGender(string genderString, Gender expectedGender)
    {
        var result = GenderExtensions.FromString(genderString);

        Assert.Equal(expectedGender, result);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData(" ")]
    public void FromString_InvalidGenderStrings_ThrowsException(string invalidGenderString)
    {
        Assert.Throws<BusinessRuleValidationException>(() => GenderExtensions.FromString(invalidGenderString));
    }

    [Theory]
    [InlineData("unknown")]
    [InlineData("123")]
    [InlineData("x")]
    public void FromString_UnknownGenderStrings_ReturnsOther(string unknownGenderString)
    {
        var result = GenderExtensions.FromString(unknownGenderString);

        Assert.Equal(Gender.Other, result);
    }
}

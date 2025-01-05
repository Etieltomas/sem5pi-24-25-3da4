using Sempi5.Domain.Shared;

namespace Sempi5.Domain.PatientEntity
{
    public enum Gender
    {
        Male,
        Female,
        Other 
    }
    
    /// <summary>
    /// @author Simão Lopes
    /// @date 1/12/2024
    /// Provides extension methods for the Gender enum.
    /// Specifically, the FromString method, which converts a string representation of gender
    /// (case-insensitive and whitespace-agnostic) into a corresponding Gender enum value.
    /// If the string does not match any known gender, it defaults to "Other".
    /// </summary>
    public static class GenderExtensions
    {
        public static Gender FromString(string genderString)
        {
            genderString = genderString?.Replace(" ", string.Empty).ToLower();
            if (string.IsNullOrEmpty(genderString))
            {
                throw new BusinessRuleValidationException("Gender cannot be null or empty.");
            }

            switch (genderString)
            {
                case "male":
                    return Gender.Male;
                case "female":
                    return Gender.Female;
                default:
                    return Gender.Other;
            }
        }
    }
}

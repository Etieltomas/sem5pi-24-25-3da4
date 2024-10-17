using Sempi5.Domain.Shared;

namespace Sempi5.Domain.Patient
{
    public enum Gender
    {
        Male,
        Female,
        Other,
    }

    public static class GenderExtensions
    {
        public static Gender FromString(string genderString)
        {
            if (string.IsNullOrEmpty(genderString))
            {
                throw new BusinessRuleValidationException("Gender cannot be null or empty.");
            }

            switch (genderString.ToLower())
            {
                case "male":
                    return Gender.Male;
                case "female":
                    return Gender.Female;
                case "other":
                    return Gender.Other;
                default:
                    throw new BusinessRuleValidationException("Gender must exits in the list.");

            }
        }
    }
}

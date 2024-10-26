using Sempi5.Domain.Shared;

namespace Sempi5.Domain.PatientEntity
{
    public enum Gender
    {
        Male,
        Female,
        Other 
    }

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

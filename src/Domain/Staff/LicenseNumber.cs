using Sempi5.Domain.Shared;

namespace Sempi5.Domain.Staff{
    public class LicenseNumber : IValueObject
    {
        private string _value { get; }

        // TODO : Maybe some business rules should be added here
        public LicenseNumber(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new BusinessRuleValidationException("License number cannot be null or empty.");
            }

            _value = value;
        }

        public override string ToString()
        {
            return _value;
        }
    }
}
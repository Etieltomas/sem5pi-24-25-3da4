using Sempi5.Domain.Shared;

namespace Sempi5.Domain.StaffEntity
{
    public class LicenseNumber : IValueObject
    {
        private string _value { get; }

        /// <summary>
        /// Initializes a new instance of the LicenseNumber class.
        /// The license number cannot be null or empty.
        /// </summary>
        /// <param name="value">The license number to be validated.</param>
        /// <exception cref="BusinessRuleValidationException">Thrown when the value is invalid.</exception>
        public LicenseNumber(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new BusinessRuleValidationException("License number cannot be null or empty.");
            }

            _value = value;
        }

        /// <summary>
        /// Returns the string representation of the LicenseNumber.
        /// </summary>
        /// <returns>The license number as a string.</returns>
        public override string ToString()
        {
            return _value;
        }
    }
}

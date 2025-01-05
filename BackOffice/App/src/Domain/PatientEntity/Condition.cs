using Sempi5.Domain.Shared;

namespace Sempi5.Domain.PatientEntity{
    /// <summary>
    /// @author Simão Lopes
    /// @date 1/12/2024
    /// Represents a medical condition associated with a patient.
    /// This class implements the IValueObject interface to define a condition as a value object,
    /// which ensures that the condition is immutable and valid (not null or empty) upon creation.
    /// The condition is stored as a string and can be converted to its string representation 
    /// via the ToString method.
    /// </summary>
    public class Condition : IValueObject
    {
        private string _value { get; }

        public Condition(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new BusinessRuleValidationException("Condition cannot be null or empty.");
            }

            _value = value;
        }

        public override string ToString()
        {
            return _value;
        }
    }
}
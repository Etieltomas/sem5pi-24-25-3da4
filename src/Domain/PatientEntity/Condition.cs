using Sempi5.Domain.Shared;

namespace Sempi5.Domain.PatientEntity{
    public class Condition : IValueObject
    {
        private string _value { get; }

        // TODO : Maybe some business rules should be added here
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
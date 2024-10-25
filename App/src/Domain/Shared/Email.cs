using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Sempi5.Domain.Shared;

public class Email : IValueObject
    {
        private readonly string _value;

        private static readonly Regex EmailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

        [JsonConstructor]
        public Email(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new BusinessRuleValidationException("Email cannot be null or empty.");
            }

            if (!EmailRegex.IsMatch(value))
            {
                throw new BusinessRuleValidationException("Email is not in a valid format.");
            }

            _value = value;
        }

        public bool Equals(Email obj)
        {
            if (obj is null) return false;
            return _value.Equals(obj._value);
        }

        public override string ToString()
        {
            return _value;
        }
    }

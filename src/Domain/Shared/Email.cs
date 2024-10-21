using Sempi5.Domain.Shared;

public class Email : IValueObject
{
    private string _value;

    // TODO : Maybe some business rules should be added here
    public Email(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new BusinessRuleValidationException("Email cannot be null or empty.");
        }

        _value = value;
    }

    public bool Equals(Email obj)
    {
        return obj._value == _value;
    }

    public override string ToString()
    {
        return _value;
    }
}
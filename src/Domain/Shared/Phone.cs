using Sempi5.Domain.Shared;

public class Phone : IValueObject
{
    private string _value { get; }

    // TODO : Maybe some business rules should be added here
    public Phone(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new BusinessRuleValidationException("Phone cannot be null or empty.");
        }

        _value = value;
    }

    public override string ToString()
    {
        return _value;
    }
}
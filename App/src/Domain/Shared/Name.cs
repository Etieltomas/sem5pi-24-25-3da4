using Sempi5.Domain.Shared;

public class Name : IValueObject
{
    private string _value { get; }

    // TODO : Maybe some business rules should be added here
    public Name(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new BusinessRuleValidationException("Name cannot be null or empty.");
        }

        _value = value;
    }

    public string FirstName()
    {
        return _value.Split(' ')[0];
    }

    public string LastName()
    {
        var names = _value.Split(' ');

        if (names.Length == 1)
        {
            return names[0];
        } 
            
        return names[names.Length - 1];    
    }

    public override string ToString()
    {
        return _value;
    }
}
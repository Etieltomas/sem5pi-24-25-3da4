using Sempi5.Domain.Shared;
using Sempi5.Domain.Staff;

public class AvailabilitySlot : IValueObject
{
    private string _value;

    // TODO : Maybe some business rules should be added here
    public AvailabilitySlot(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new BusinessRuleValidationException("AvailabilitySlot cannot be null or empty.");
        }

        _value = value;
    }

    public override string ToString()
    {
        return _value;
    }
}
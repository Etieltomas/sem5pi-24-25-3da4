using Sempi5.Domain.Shared;
using Sempi5.Domain.StaffEntity;

public class AvailabilitySlot : IValueObject
{
    private string _value;

    // Example: 2024-10-21T09:00:00 - 2024-10-21T11:00:00,
    public AvailabilitySlot(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new BusinessRuleValidationException("AvailabilitySlot cannot be null or empty.");
        }

        var split = value.Split(" - ");
        if (split.Length != 2)
        {
            throw new BusinessRuleValidationException("AvailabilitySlot must have a start and end date.");
        }

        var start = DateTime.Parse(split[0]);
        var end = DateTime.Parse(split[1]);

        if (start >= end)
        {
            throw new BusinessRuleValidationException("AvailabilitySlot start date must be before end date.");
        }

        _value = value;
    }

    public override string ToString()
    {
        return _value;
    }
}
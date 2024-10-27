using System.Globalization;
using Sempi5.Domain.Shared;
using Sempi5.Domain.StaffEntity;

public class AvailabilitySlot : IValueObject
{
    private string _value;

    // Example: 21-10-2024T09:00:00 - 21-10-2024T11:00:00,
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

        var start = DateTime.ParseExact(split[0], "dd-MM-yyyyTHH:mm:ss", CultureInfo.InvariantCulture);
        var end = DateTime.ParseExact(split[1], "dd-MM-yyyyTHH:mm:ss", CultureInfo.InvariantCulture);

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
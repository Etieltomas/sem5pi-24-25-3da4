using System.Globalization;
using Sempi5.Domain.Shared;

public class Slot: IValueObject
{
    private string _value;

    // Example: 21-10-2024T09:00:00 - 21-10-2024T11:00:00,
    public Slot(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new BusinessRuleValidationException("Slot cannot be null or empty.");
        }

        var split = value.Split(" - ");
        if (split.Length != 2)
        {
            throw new BusinessRuleValidationException("Slot must have a start and end date.");
        }

        var start = DateTime.ParseExact(split[0], "dd-MM-yyyyTHH:mm:ss", CultureInfo.InvariantCulture);
        var end = DateTime.ParseExact(split[1], "dd-MM-yyyyTHH:mm:ss", CultureInfo.InvariantCulture);

        if (start >= end)
        {
            throw new BusinessRuleValidationException("Slot start date must be before end date.");
        }

        if (start < DateTime.Now || end < DateTime.Now)
        {
            throw new BusinessRuleValidationException("Slot start and end date must be in the future.");
        }


        _value = value;
    }

    public override string ToString()
    {
        return _value;
    }
}
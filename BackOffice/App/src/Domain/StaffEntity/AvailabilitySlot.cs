using System.Globalization;
using Sempi5.Domain.Shared;
using Sempi5.Domain.StaffEntity;

public class AvailabilitySlot : IValueObject
{
    private string _value;

    /// <summary>
    /// Initializes a new instance of the AvailabilitySlot class.
    /// The value should represent a time slot in the format "dd-MM-yyyyTHH:mm:ss - dd-MM-yyyyTHH:mm:ss".
    /// </summary>
    /// <param name="value">The availability slot in the format "start date - end date".</param>
    /// <example>
    /// Example: "21-10-2024T09:00:00 - 21-10-2024T11:00:00"
    /// </example>
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

        if (start < DateTime.Now || end < DateTime.Now)
        {
            throw new BusinessRuleValidationException("AvailabilitySlot start and end date must be in the future.");
        }

        _value = value;
    }

    /// <summary>
    /// Returns the string representation of the AvailabilitySlot.
    /// </summary>
    /// <returns>The availability slot in string format.</returns>
    public override string ToString()
    {
        return _value;
    }
}

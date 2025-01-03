using System.Globalization;
using Sempi5.Domain.Shared;

public class Slot : IValueObject
{
    private string _value;

    /// <summary>
    /// Constructor for the Slot class.
    /// Validates the input string for proper slot format and future dates.
    /// @author Tomás Leite
    /// @date 30/11/2024
    /// </summary>
    /// <param name="value">The slot value in the format "dd-MM-yyyyTHH:mm:ss - dd-MM-yyyyTHH:mm:ss".</param>
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

    /// <summary>
    /// Returns the string representation of the slot.
    /// @author Tomás Leite
    /// @date 30/11/2024
    /// </summary>
    /// <returns>The slot value as a string.</returns>
    public override string ToString()
    {
        return _value;
    }
}

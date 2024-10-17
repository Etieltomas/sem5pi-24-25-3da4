using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

public class AvailabilitySlotListConverter : ValueConverter<List<AvailabilitySlot>, string>
{
    public AvailabilitySlotListConverter() : base(
        v => string.Join(',', v.Select(slot => slot.ToString())),
        v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(slot => new AvailabilitySlot(slot)).ToList())
    {
    }
}

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Sempi5.Domain.StaffEntity
{
    /// <summary>
    /// Converts a list of AvailabilitySlot objects to a string representation and vice versa.
    /// Used for persisting AvailabilitySlot lists as strings in the database.
    /// </summary>
    public class AvailabilitySlotListConverter : ValueConverter<List<AvailabilitySlot>, string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AvailabilitySlotListConverter"/> class.
        /// </summary>
        public AvailabilitySlotListConverter() : base(
            // Converts a list of AvailabilitySlot objects to a comma-separated string.
            v => string.Join(',', v.Select(slot => slot.ToString())),

            // Converts a comma-separated string back into a list of AvailabilitySlot objects.
            v => v.Split(',', StringSplitOptions.RemoveEmptyEntries)
                  .Select(slot => new AvailabilitySlot(slot))
                  .ToList())
        {
        }
    }
}

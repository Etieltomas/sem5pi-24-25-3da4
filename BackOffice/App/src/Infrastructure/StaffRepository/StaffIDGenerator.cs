using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using Sempi5.Domain.StaffEntity;
using Sempi5.Infrastructure.Databases;

public class StaffIDGenerator : ValueGenerator<StaffID>
{
    /// <summary>
    /// Generates the next StaffID based on the staff's email and current year, ensuring sequential numbering.
    /// @actor: Tomás Leite
    /// @date: 30/11/2024
    /// </summary>
    /// <param name="entry">The entry of the staff entity for which the ID is being generated.</param>
    /// <returns>The newly generated StaffID.</returns>
    public override StaffID Next(EntityEntry entry)
    {
        var context = (DataBaseContext)entry.Context;

        var currentYear = DateTime.Now.ToString("yyyy");

        var staffType = entry.Property("Email").CurrentValue.ToString().ToUpper().ToArray()[0];

        var latestNumber = context.Staff
            .AsEnumerable()
            .Where(s => s.Id.Value.StartsWith($"{staffType}{currentYear}"))
            .OrderByDescending(s => s.Id)
            .Select(s => s.Id)
            .FirstOrDefault();

        var sequentialNumber = latestNumber != null ? int.Parse(latestNumber.Value.Substring(6)) + 1 : 1;

        var newNumber = $"{staffType}{currentYear}{sequentialNumber:D5}";

        entry.Property("Id").CurrentValue = new StaffID(newNumber);

        return new StaffID(newNumber);
    }

    /// <summary>
    /// Indicates whether temporary values are generated for the StaffID.
    /// @actor: Tomás Leite
    /// @date: 30/11/2024
    /// </summary>
    public override bool GeneratesTemporaryValues => false;
}

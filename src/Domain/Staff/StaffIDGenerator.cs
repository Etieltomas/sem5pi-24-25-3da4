using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using Sempi5.Domain.Staff;
using Sempi5.Infrastructure.Databases;

public class StaffIDGenerator : ValueGenerator<StaffID>
{
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

    public override bool GeneratesTemporaryValues => false;
}

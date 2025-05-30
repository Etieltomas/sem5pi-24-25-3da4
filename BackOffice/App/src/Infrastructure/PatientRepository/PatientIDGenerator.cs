using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using Sempi5.Domain.PatientEntity;
using Sempi5.Infrastructure.Databases;

public class PatientIDGenerator : ValueGenerator<PatientID>
{
    /// <summary>
    /// Configuration class for the Patient entity. This class configures the mapping 
    /// of the Patient class to the database table and sets the rules for its properties.
    /// </summary>
    /// @author Simão Lopes
    /// @date 5/12/2024
    public override PatientID Next(EntityEntry entry)
    {
        var context = (DataBaseContext) entry.Context;

        var currentMonthPrefix = DateTime.Now.ToString("yyyyMM");

        var latestNumber = context.Patients
            .AsEnumerable() 
            .Where(p => p.Id.Value.StartsWith(currentMonthPrefix))
            .OrderByDescending(p => p.Id)
            .Select(p => p.Id)
            .FirstOrDefault();

        var sequentialNumber = latestNumber != null ? int.Parse(latestNumber.Value.Substring(6)) + 1 : 1;

        var newNumber = currentMonthPrefix + sequentialNumber.ToString("D6");

        entry.Property("Id").CurrentValue = new PatientID(newNumber);

        return new PatientID(newNumber);
    }

    public override bool GeneratesTemporaryValues => false;
}

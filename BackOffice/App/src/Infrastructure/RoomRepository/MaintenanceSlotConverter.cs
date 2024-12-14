using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Sempi5.Domain.PatientEntity;

public class MaintenanceSlotConverter : ValueConverter<List<Slot>, string>
{
    public MaintenanceSlotConverter() : base(
        v => string.Join(',', v.Select(cond => cond.ToString())),
        v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(cond => new Slot(cond)).ToList())
    {
    }
}

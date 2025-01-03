using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

public class MaintenanceSlotConverter : ValueConverter<List<Slot>, string>
{
    /// <summary>
    /// Converts a list of Slot objects into a comma-separated string and vice versa.
    /// @author Tomás Leite
    /// @date 30/11/2024
    /// </summary>
    public MaintenanceSlotConverter() : base(
        v => string.Join(',', v.Select(cond => cond.ToString())),
        
        v => v.Split(',', StringSplitOptions.RemoveEmptyEntries)
              .Select(cond => new Slot(cond))
              .ToList())
    {}
}

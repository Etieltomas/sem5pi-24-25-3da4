using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Sempi5.Domain.RoomEntity;

/// <summary>
/// Custom value converter for converting AssignedEquipment to a string and vice versa.
/// @auth Tomás Leite
/// @date 30/11/2024
/// </summary>
public class EquipmentConverter : ValueConverter<AssignedEquipment, string>
{
    /// <summary>
    /// Constructor for EquipmentConverter.
    /// Converts AssignedEquipment to a comma-separated string during saving and parses the string back into AssignedEquipment when loading.
    /// @auth Tomás Leite
    /// @date 30/11/2024
    /// </summary>
    public EquipmentConverter() : base(
        v => string.Join(',', v.equipment.Select(cond => cond.ToString())),
        v => ParseToAssignedEquipment(v)
    )
    {
    }

    /// <summary>
    /// Converts a comma-separated string into an AssignedEquipment object.
    /// @auth Tomás Leite
    /// @date 30/11/2024
    /// </summary>
    /// <param name="v">The input string to parse.</param>
    /// <returns>An AssignedEquipment object with the parsed list of equipment.</returns>
    private static AssignedEquipment ParseToAssignedEquipment(string v)
    {
        var list = v.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(cond => cond).ToList();
        return new AssignedEquipment(list);
    }
}

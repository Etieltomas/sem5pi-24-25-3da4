using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.WebEncoders.Testing;
using Sempi5.Domain.PatientEntity;
using Sempi5.Domain.RoomEntity;

public class EquipmentConverter : ValueConverter<AssignedEquipment, string>
{
    public EquipmentConverter() : base(
        v => string.Join(',', v.equipment.Select(cond => cond.ToString())),
        v => teste(v)
    )
    {
    }

    private static AssignedEquipment teste(string v)
    {
        var list = v.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(cond => cond).ToList();
        
        return new AssignedEquipment(list);
    }
}

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Sempi5.Domain.Patient;

public class ConditionListConverter : ValueConverter<List<Condition>, string>
{
    public ConditionListConverter() : base(
        v => string.Join(',', v.Select(cond => cond.ToString())),
        v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(cond => new Condition(cond)).ToList())
    {
    }
}

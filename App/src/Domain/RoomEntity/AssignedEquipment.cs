using System.Text;
using Sempi5.Domain.Shared;

namespace Sempi5.Domain.RoomEntity {
    public class AssignedEquipment : IValueObject
    {
        public List<string> equipment;

        public AssignedEquipment(List<string> equipment)
        {
            if (equipment == null || equipment.Count == 0)
            {
                throw new BusinessRuleValidationException("Equipment cannot neither be null nor empty.");
            }
            this.equipment = equipment;
        }

        public bool Equals(AssignedEquipment obj)
        {
            return obj.equipment == equipment;
        }

        public override string ToString()
        {
            var stringBuilder = new StringBuilder();
            foreach (var item in equipment)
            {
                stringBuilder.Append(item);
                stringBuilder.Append(", ");
            }
            return stringBuilder.ToString();
        }
    }
}
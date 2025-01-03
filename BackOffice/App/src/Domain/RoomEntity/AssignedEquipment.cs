using System.Text;
using Sempi5.Domain.Shared;

namespace Sempi5.Domain.RoomEntity {
    public class AssignedEquipment : IValueObject
    {
        public List<string> equipment;

        /// <summary>
        /// Constructor for AssignedEquipment.
        /// Ensures the equipment list is not null or empty.
        /// @actor: Tomás Leite
        /// @date: 30/11/2024
        /// </summary>
        /// <param name="equipment">List of equipment to assign.</param>
        public AssignedEquipment(List<string> equipment)
        {
            if (equipment == null || equipment.Count == 0)
            {
                throw new BusinessRuleValidationException("Equipment cannot neither be null nor empty.");
            }
            this.equipment = equipment;
        }

        /// <summary>
        /// Checks equality between two AssignedEquipment objects based on their equipment lists.
        /// @actor: Tomás Leite
        /// @date: 30/11/2024
        /// </summary>
        /// <param name="obj">The AssignedEquipment object to compare with.</param>
        /// <returns>True if the equipment lists are the same; otherwise, false.</returns>
        public bool Equals(AssignedEquipment obj)
        {
            return obj.equipment == equipment;
        }

        /// <summary>
        /// Converts the equipment list to a comma-separated string.
        /// @actor: Tomás Leite
        /// @date: 30/11/2024
        /// </summary>
        /// <returns>A string representation of the equipment list.</returns>
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

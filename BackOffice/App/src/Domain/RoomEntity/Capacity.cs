using Sempi5.Domain.Shared;

namespace Sempi5.Domain.RoomEntity {
    public class Capacity : IValueObject
    {
        private int capacity;

        /// <summary>
        /// Constructor for Capacity class.
        /// Ensures the capacity is non-negative.
        /// @author Tomás Leite
        /// @date 28/11/2024
        /// </summary>
        /// <param name="capacity">The capacity value.</param>
        public Capacity(int capacity)
        {
            if (capacity < 0)
            {
                throw new BusinessRuleValidationException("Capacity cannot be negative.");
            }

            this.capacity = capacity;
        }

        /// <summary>
        /// Compares two Capacity objects for equality.
        /// @author Tomás Leite
        /// @date 28/11/2024
        /// </summary>
        /// <param name="obj">The other Capacity object to compare.</param>
        /// <returns>True if capacities are equal, false otherwise.</returns>
        public bool Equals(Capacity obj)
        {
            return obj.capacity == capacity;
        }

        /// <summary>
        /// Converts the capacity value to a string.
        /// @author Tomás Leite
        /// @date 28/11/2024
        /// </summary>
        /// <returns>String representation of the capacity.</returns>
        public override string ToString()
        {
            return capacity.ToString();
        }

        /// <summary>
        /// Retrieves the capacity as an integer.
        /// @author Tomás Leite
        /// @date 28/11/2024
        /// </summary>
        /// <returns>Integer value of the capacity.</returns>
        public int AsInt()
        {
            return capacity;
        }
    }
}

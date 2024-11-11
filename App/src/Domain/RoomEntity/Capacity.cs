using Sempi5.Domain.Shared;

namespace Sempi5.Domain.RoomEntity {
    public class Capacity : IValueObject
    {
        private int capacity;

        public Capacity(int capacity)
        {
            if (capacity < 0)
            {
                throw new BusinessRuleValidationException("Capacity cannot be negative.");
            }

            this.capacity = capacity;
        }

        public bool Equals(Capacity obj)
        {
            return obj.capacity == capacity;
        }

        public override string ToString()
        {
            return capacity.ToString();
        }
    }
}
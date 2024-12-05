using Sempi5.Domain.Shared;

namespace Sempi5.Domain.AllergyEntity{
    public class Code : IValueObject
    {
        private int _value { get; }

        public Code(int value)
        {
            _value = value;
        }

        public int ToInt()
        {
            return _value;
        }
    }
}
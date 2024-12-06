using System.Text.Json.Serialization;
using Sempi5.Domain.Shared;

namespace Sempi5.Domain.AllergyEntity{
    public class Code : IValueObject
    {
        [JsonPropertyName("value")]
        private int _value;

        [JsonConstructor]
        public Code(int value)
        {
            _value = value;
        }

        public virtual int ToInt()
        {
            return _value;
        }
    }
}
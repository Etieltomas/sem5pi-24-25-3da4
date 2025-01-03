using System;
using Sempi5.Domain.Shared;

namespace Sempi5.Domain.OperationRequestEntity
{
    /**
     * Priority.cs created by Ricardo Guimarães on 10/12/2024
     */
    public class Priority : ValueObject
    {
        public virtual string Value { get; }

        private Priority(string value)
        {
            Value = value;
        }

        public static Priority High => new Priority("High");
        public static Priority Medium => new Priority("Medium");
        public static Priority Low => new Priority("Low");

        public static Priority FromString(string value)
        {
            if (value != "High" && value != "Medium" && value != "Low")
                throw new ArgumentException("Invalid priority value");

            return new Priority(value);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
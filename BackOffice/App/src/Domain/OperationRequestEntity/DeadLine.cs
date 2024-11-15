using System;
using System.Collections.Generic;
using Sempi5.Domain.Shared;

namespace Sempi5.Domain.OperationRequestEntity
{
    public class Deadline : ValueObject
    {
        public DateTime Value { get; }

        public Deadline(DateTime value)
        {
            if (value <= DateTime.Now)
                throw new ArgumentException("Deadline must be a future date");

            Value = value;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
using System;
using System.Collections.Generic;
using Sempi5.Domain.Shared;

namespace Sempi5.Domain.OperationRequestEntity
{
    /**
     * Deadline.cs created by Ricardo Guimarães on 10/12/2024
     */
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
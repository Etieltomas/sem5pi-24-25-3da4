using System;
using System.Collections.Generic;
using Sempi5.Domain.Shared;

namespace Sempi5.Domain.OperationRequestEntity
{
    public class Status : ValueObject
    {
        public string Value { get; }

        private Status(string value)
        {
            Value = value;
        }

        public static Status Pending => new Status("Pending");
        public static Status scheduled => new Status("Scheduled");
        public static Status Completed => new Status("Completed");
        public static Status Cancelled => new Status("Cancelled");

        public static Status FromString(string value)
        {
            if (value != "Pending" && value != "Scheduled" && value != "Completed" && value != "Cancelled")
                throw new ArgumentException("Invalid status value");

            return new Status(value);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}


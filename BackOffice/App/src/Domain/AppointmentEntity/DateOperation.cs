using Sempi5.Domain.Shared;

namespace Sempi5.Domain.AppointmentEntity
{
    public class DateOperation : ValueObject
    {
        public DateTime Value { get; }

        public DateOperation(DateTime value)
        {
            if (value <= DateTime.Now)
                throw new ArgumentException("Date of operation must be a future date");

            Value = value;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
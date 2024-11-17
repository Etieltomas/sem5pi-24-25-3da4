using Sempi5.Domain.Shared;

namespace Sempi5.Domain.AppointmentEntity
{
    public class AppointmentID : EntityId
    {
        
        public AppointmentID(long value) : base(value)
        {
        }
        
        protected override object createFromString(string text)
        {
            return long.Parse(text);
        }

        public override string AsString()
        {
            return (string)base.ObjValue.ToString();
        }
        
        public long AsLong()
        {
            return (long)base.ObjValue;
        }
    }
}
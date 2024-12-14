using Sempi5.Domain.Shared;

namespace Sempi5.Domain.OperationRequestEntity
{
    public class OperationRequestID : EntityId
    {
        public OperationRequestID(long value) : base(value) { }

        protected override object createFromString(string text)
        {
            return text;
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


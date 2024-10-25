using Sempi5.Domain.Shared;

namespace Sempi5.Domain.OperationRequestEntity
{
    public class OperationTypeID : EntityId
    {
        public OperationTypeID(long value) : base(value) { }

        protected override object createFromString(string text)
        {
            return text;
        }

        public override string AsString()
        {
            return (string)ObjValue;
        }

        public long AsLong()
        {
            return (long)ObjValue;
        }
    }
}


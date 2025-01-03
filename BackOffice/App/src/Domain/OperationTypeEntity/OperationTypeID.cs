using Sempi5.Domain.Shared;

namespace Sempi5.Domain.OperationRequestEntity
{
    public class OperationTypeID : EntityId
    {
        /**
         * OperationTypeID.cs created by Ricardo Guimarães on 10/12/2024
         */
        public OperationTypeID(long value) : base(value) { }

        protected override object createFromString(string text)
        {
            return text;
        }

        public override string AsString()
        {
            return ObjValue.ToString();
        }

        public long AsLong()
        {
            return (long)ObjValue;
        }
    }
}


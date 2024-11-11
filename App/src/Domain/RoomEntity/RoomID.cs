using System;
using Newtonsoft.Json;
using Sempi5.Domain.Shared;

namespace Sempi5.Domain.RoomEntity
{
    public class RoomID : EntityId
    {
        
        public RoomID(long value) : base(value)
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
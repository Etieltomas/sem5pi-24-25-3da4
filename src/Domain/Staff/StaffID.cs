using System;
using Newtonsoft.Json;
using Sempi5.Domain.Shared;

namespace Sempi5.Domain.Staff
{
    public class StaffID : EntityId
    {
        
        public StaffID() : base(null)
        {
        }
        
        public StaffID(string value) : base(value)
        {
        }
        
        protected override object createFromString(string text)
        {
            return text;
        }

        override
        public string AsString()
        {
            return (string)ObjValue;
        }
    }
}
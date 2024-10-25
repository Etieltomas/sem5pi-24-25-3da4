using System;
using Newtonsoft.Json;
using Sempi5.Domain.Shared;

namespace Sempi5.Domain.SpecializationEntity
{
    public class SpecializationID : EntityId
    {
        
        public SpecializationID(string value) : base(value)
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

        public bool Equals(SpecializationID other)
        {
            return ObjValue.ToString().Equals(other.ObjValue.ToString());
        }
    }
}
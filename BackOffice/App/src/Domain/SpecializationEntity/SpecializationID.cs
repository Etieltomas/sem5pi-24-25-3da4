using System;
using Newtonsoft.Json;
using Sempi5.Domain.Shared;

namespace Sempi5.Domain.SpecializationEntity
{
    public class SpecializationID : EntityId
    {
        /**
         * SpecializationID.cs created by Ricardo Guimarães on 10/12/2024
         */
        
        public SpecializationID(long value) : base(value)
        {
        }
        
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


        public bool Equals(SpecializationID other)
        {
            return ObjValue.ToString().Equals(other.ObjValue.ToString());
        }
    }
}
using Sempi5.Domain.Shared;

namespace Sempi5.Domain.TokenEntity
{
    public class TokenID : EntityId
    {
        public TokenID(Guid value) : base(value)
        {
        }

        public TokenID(String value) : base(value)
        {
        }

        
        protected override Object createFromString(String text){
            return new Guid(text);
        }

        override
        public String AsString(){
            Guid obj = (Guid) base.ObjValue;
            return obj.ToString();
        }
        
       
        public Guid AsGuid(){
            return (Guid) base.ObjValue;
        }

    }
}
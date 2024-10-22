using Sempi5.Domain.Shared;

namespace Sempi5.Domain.Token
{
    public class TokenDTO 
    {
        public string TokenValue { get; set; }
        public string Email { get; set; }    
        public string ExpirationDate { get; set; }  
        public bool IsUsed { get; set; }
    }

}
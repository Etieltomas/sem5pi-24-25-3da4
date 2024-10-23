using Sempi5.Domain.Shared;

namespace Sempi5.Domain.Token
{
    public class Token : Entity<TokenID>
    {
        public Email Email { get; set; }    
        public DateTime ExpirationDate { get; set; }  
        public bool IsUsed { get; set; }
    }

}
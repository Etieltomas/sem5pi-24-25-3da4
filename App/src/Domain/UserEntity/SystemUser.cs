using Sempi5.Domain.Shared;

namespace Sempi5.Domain.UserEntity
{
    public class SystemUser : Entity<SystemUserId>
    {
        public virtual string Username { get; set; }
        public virtual string Role { get; set; }
        public virtual Email Email { get; set; }    
        public virtual bool Active { get; set; }
        public virtual bool MarketingConsent { get; set; } = false;

    
    }
}
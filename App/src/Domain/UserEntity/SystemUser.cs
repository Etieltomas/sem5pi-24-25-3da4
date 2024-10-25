using Sempi5.Domain.Shared;

namespace Sempi5.Domain.UserEntity
{
    public class SystemUser : Entity<SystemUserId>
    {
        public string Username { get; set; }
        public string Role { get; set; }
        public Email Email { get; set; }    
        public bool Active { get; set; }
        public bool MarktingConsent { get; set; } = false;

    }
}
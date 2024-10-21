using Sempi5.Domain.Shared;

namespace Sempi5.Domain.User
{
    public class SystemUser : Entity<SystemUserId>
    {
        public SystemUserId Id { get; set; }
        public string Username { get; set; }
        public string Role { get; set; }
        //public Email IAMEmail { get; set; }  
        public Email Email { get; set; }    
        public bool Active { get; set; }  
    }
}
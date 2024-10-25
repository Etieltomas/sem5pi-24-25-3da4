using Sempi5;

namespace Sempi5.Domain.UserEntity
{
    public class SystemUserDTO
    {
        public string? Id { get; set; }
        public string Username { get; set; }
        public string Role { get; set; }
        public string Email { get; set; }    
        public bool Active { get; set; }
        public bool MarktingConsent { get; set; }
    }
}
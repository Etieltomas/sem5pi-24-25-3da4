using System.Security.Claims;
using Sempi5;
using Sempi5.Domain.Shared;
using Sempi5.Domain.User;

namespace Sempi5.Domain.Staff
{
    public class Staff : IAggregateRoot
    {
        // TODO: Change some of this variable to the
        //              real class instead of string
        public long LicenseNumber { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }  
        public List<string> AvailabilitySlots { get; set; }  
        public string Specialization { get; set; }
        public SystemUser? SystemUser { get; set; }
    }
}
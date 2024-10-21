using System.Security.Claims;
using Sempi5;
using Sempi5.Domain.Shared;
using Sempi5.Domain.SpecializationEntity;
using Sempi5.Domain.User;

namespace Sempi5.Domain.Staff
{
    public class Staff : Entity<StaffID>, IAggregateRoot
    {
        // TODO: Missing Specialization Class
        public StaffID Id { get; set; }
        public LicenseNumber LicenseNumber { get; set; }
        public Name Name { get; set; }
        public Email Email { get; set; }
        public Phone Phone { get; set; }  
        public Address Address { get; set; }
        public List<AvailabilitySlot> AvailabilitySlots { get; set; }  
        public Specialization Specialization { get; set; }
        public SystemUser SystemUser { get; set; }
    }
}
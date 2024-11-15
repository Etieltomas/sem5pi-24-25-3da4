using System.Security.Claims;
using Sempi5;
using Sempi5.Domain.Shared;
using Sempi5.Domain.SpecializationEntity;
using Sempi5.Domain.UserEntity;

namespace Sempi5.Domain.StaffEntity
{
    public class Staff : Entity<StaffID>, IAggregateRoot
    {
        public virtual LicenseNumber LicenseNumber { get; set; }
        public virtual Name Name { get; set; }
        public virtual Email Email { get; set; }
        public virtual Phone Phone { get; set; }  
        public virtual Address Address { get; set; }
        public virtual List<AvailabilitySlot> AvailabilitySlots { get; set; }  
        public virtual Specialization Specialization { get; set; }
        public virtual SystemUser SystemUser { get; set; }
    }
}
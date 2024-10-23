using Sempi5.Domain.Shared;
using Sempi5.Domain.UserEntity;

namespace Sempi5.Domain.PatientEntity
{
    public class Patient : Entity<PatientID>, IAggregateRoot
    {
        public DateTime DateOfBirth { get; set; }
        public Name Name { get; set; }
        public Gender Gender { get; set; }
        public Email Email { get; set; }
        public Phone Phone { get; set; }  
        public Address Address { get; set; }
        public List<Condition> Conditions { get; set; }  
        public Phone EmergencyContact { get; set; }
        public SystemUser? SystemUser { get; set; }
    }
}
using Sempi5.Domain.Shared;
using Sempi5.Domain.User;

namespace Sempi5.Domain.Patient
{
    public class Patient : Entity<PatientID>, IAggregateRoot
    {
        // TODO: Change some of this variable to the
        //              real class instead of string
        public PatientID Id { get; set; }
        public DateTime DateOfBirth { get; set; }
        public Name Name { get; set; }
        public Gender Gender { get; set; }
        public Email Email { get; set; }
        public Phone Phone { get; set; }  
        public List<Condition> Conditions { get; set; }  
        public Phone EmergencyContact { get; set; }
        public SystemUser? SystemUser { get; set; }
    }
}
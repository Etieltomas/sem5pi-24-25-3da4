using System.Security.Claims;
using Sempi5;
using Sempi5.Domain.Shared;
using Sempi5.Domain.User;

namespace Sempi5.Domain.Patient
{
    public class Patient : IAggregateRoot
    {
        // TODO: Change some of this variable to the
        //              real class instead of string
        public long MedicalRecordNumber { get; set; }
        public string DateOfBirth { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }  
        public List<string> Conditions { get; set; }  
        public string EmergencyContact { get; set; }
        public SystemUser SystemUser { get; set; }
    }
}
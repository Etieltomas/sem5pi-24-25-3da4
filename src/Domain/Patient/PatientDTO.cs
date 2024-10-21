using Sempi5;

namespace Sempi5.Domain.Patient
{
    public class PatientDTO
    {        
        public string? MedicalRecordNumber { get; set; }
        public string DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }  
        public string Address { get; set; }
        public List<string> Conditions { get; set; }  
        public string EmergencyContact { get; set; }
    }
}
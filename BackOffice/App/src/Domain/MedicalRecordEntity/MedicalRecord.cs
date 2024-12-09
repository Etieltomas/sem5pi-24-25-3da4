using Sempi5.Domain.AllergyEntity;
using Sempi5.Domain.PatientEntity;

namespace Sempi5.Domain.MedicalRecordEntity
{
    public class MedicalRecord
    {
        public PatientID Patient { get; set; }
        public List<AllergyDTO> Allergies { get; set; }
        public List<Condition> Conditions { get; set; }

    }
}
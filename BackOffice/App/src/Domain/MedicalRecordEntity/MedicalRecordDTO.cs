using Sempi5;

namespace Sempi5.Domain.MedicalRecordEntity
{
    public class MedicalRecordDTO {
        public string Patient { get; set; }
        public List<string> Allergies { get; set; }
        public List<string> Conditions { get; set; }

    }
}
namespace Sempi5.Domain.MedicalRecordEntity
{
    public class RecordLineDTO
    {
        public string Date { get; set; }
        public string Doctor { get; set; }
        public string Type { get; set; }
        public string Designation { get; set; }
        public string? Obs { get; set; }
        public string? _id { get; set; }
    }
}
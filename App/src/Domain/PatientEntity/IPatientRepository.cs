
using Sempi5.Domain.Shared;

namespace Sempi5.Domain.PatientEntity
{
    public interface IPatientRepository : IRepository<Patient,PatientID>
    {
        public Task<Patient> GetPatientByEmail(string email);
        public Task<Patient> GetPatientById(PatientID id);
        public Task<List<Patient>> GetAllPatients();

        public Task<List<Patient>> GetPatientsFiltered(string? name, string? email, string? dateOfBirth, string? medicalRecordNumber, int page, int pageSize);
        public Task<Patient> GetPatientByName(string name);
    }
}
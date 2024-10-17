
using Sempi5.Domain.Shared;

namespace Sempi5.Domain.Patient
{
    public interface IPatientRepository : IRepository<Patient,PatientID>
    {
        public Task<Patient> GetPatientByEmail(string email);
    }
}

using Sempi5;
using Sempi5.Domain;
using Sempi5.Domain.Shared;
using Sempi5.Domain.Patient;
using Microsoft.AspNetCore.Mvc;

namespace Sempi5.Infrastructure.PatientRepository
{
    public interface IPatientRepository
    {
        public Task<PatientDTO> AddPatient(PatientDTO PatientDTO);
        public Task<Patient> GetPatientByMedicalRecordNumber(long id);
        public Task<Patient> GetPatientByEmail(string email);
        public Task<ActionResult<IEnumerable<Patient>>> GetAllPatients();

    }
}
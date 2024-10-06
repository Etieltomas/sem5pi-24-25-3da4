using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sempi5.Domain.Shared;
using Sempi5.Infrastructure.Databases;
using Sempi5.Infrastructure.PatientRepository;

namespace Sempi5.Domain.Patient
{
    public class PatientService
    {
        private readonly IPatientRepository _repo;

        public PatientService(IPatientRepository repo)
        {
            this._repo = repo;
        }

        public async Task<PatientDTO> AddPatient(PatientDTO PatientDTO)
        {
            return await _repo.AddPatient(PatientDTO);
        }

        public async Task<PatientDTO> GetPatient(long id)
        {
            return await _repo.GetPatientByMedicalRecordNumber(id);
        }

        public async Task<ActionResult<IEnumerable<PatientDTO>>> GetAllPatients()
        {
            return await _repo.GetAllPatients();
        }
    }
}
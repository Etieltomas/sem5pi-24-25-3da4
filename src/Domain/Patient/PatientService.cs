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

        public async Task<PatientDTO> GetPatientByMedicalRecordNumber(long id)
        {
            var patient = await _repo.GetPatientByMedicalRecordNumber(id);

            return new PatientDTO { MedicalRecordNumber = patient.MedicalRecordNumber, Name = patient.Name, Email = patient.Email, Phone = patient.Phone, Conditions = patient.Conditions, EmergencyContact = patient.EmergencyContact, DateOfBirth = patient.DateOfBirth };
        }

        public async Task<PatientDTO> GetPatientByEmail(string email)
        {
            var patient = await _repo.GetPatientByEmail(email);

            return new PatientDTO { MedicalRecordNumber = patient.MedicalRecordNumber, Name = patient.Name, Email = patient.Email, Phone = patient.Phone, Conditions = patient.Conditions, EmergencyContact = patient.EmergencyContact, DateOfBirth = patient.DateOfBirth };
        }


        public async Task<ActionResult<IEnumerable<PatientDTO>>> GetAllPatients()
        {
            var result = await _repo.GetAllPatients();  

            if (result == null)
            {
                return null;
            }

            var list = result.Value.ToList(); 
            
            List<PatientDTO> listDto = list.ConvertAll(cat => new PatientDTO 
            { 
                MedicalRecordNumber = cat.MedicalRecordNumber, 
                Name = cat.Name, 
                Email = cat.Email, 
                Phone = cat.Phone, 
                Conditions = cat.Conditions, 
                EmergencyContact = cat.EmergencyContact, 
                DateOfBirth = cat.DateOfBirth 
            });

            return listDto;  
        }

    }
}
using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sempi5.Domain.Shared;
using Sempi5.Infrastructure.PatientRepository;

namespace Sempi5.Domain.Patient
{
    public class PatientService
    {
        private readonly IUnitOfWork _unitOfWork;

        private readonly IPatientRepository _repo;

        public PatientService(IPatientRepository repo, IUnitOfWork unitOfWork)
        {
            this._repo = repo;
            this._unitOfWork = unitOfWork;
        }

        public async Task<PatientDTO> AddPatient(PatientDTO patientDTO)
        {
            var patient = new Patient {
                Name = patientDTO.Name,
                Email = patientDTO.Email,
                Phone = patientDTO.Phone,
                Conditions = patientDTO.Conditions,
                EmergencyContact = patientDTO.EmergencyContact,
                DateOfBirth = patientDTO.DateOfBirth,
                SystemUser = null
            };
            
            await this._repo.AddAsync(patient);

            await this._unitOfWork.CommitAsync();

            var pat = await _repo.GetPatientByEmail(patientDTO.Email);

            return new PatientDTO { MedicalRecordNumber = pat.Id.Value, Name = pat.Name, Email = pat.Email, Phone = pat.Phone, Conditions = pat.Conditions, EmergencyContact = pat.EmergencyContact, DateOfBirth = pat.DateOfBirth };
        }

        public async Task<PatientDTO> GetPatientByMedicalRecordNumber(PatientID id)
        {
            var patient = await _repo.GetByIdAsync(id);

            return new PatientDTO { MedicalRecordNumber = patient.Id.Value, Name = patient.Name, Email = patient.Email, Phone = patient.Phone, Conditions = patient.Conditions, EmergencyContact = patient.EmergencyContact, DateOfBirth = patient.DateOfBirth };
        }

        public async Task<PatientDTO> GetPatientByEmail(string email)
        {
            var patient = await _repo.GetPatientByEmail(email);

            if (patient == null)
            {
                return null;
            }
            
            return new PatientDTO { MedicalRecordNumber = patient.Id.Value, Name = patient.Name, Email = patient.Email, Phone = patient.Phone, Conditions = patient.Conditions, EmergencyContact = patient.EmergencyContact, DateOfBirth = patient.DateOfBirth };
        }


        public async Task<List<PatientDTO>> GetAllPatients()
        {
            var list = await _repo.GetAllAsync();  

            if (list == null)
            {
                return null;
            }

            
            List<PatientDTO> listDto = list.ConvertAll(cat => new PatientDTO 
            { 
                MedicalRecordNumber = cat.Id.Value,
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
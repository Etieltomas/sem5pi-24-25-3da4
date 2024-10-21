using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            _repo = repo;
            _unitOfWork = unitOfWork;
        }

        public async Task<PatientDTO> AddPatient(PatientDTO patientDTO)
        {
            var address = patientDTO.Address.Split(", ");
            var patient = new Patient
            {
                Gender = GenderExtensions.FromString(patientDTO.Gender.ToLower()),
                Name = new Name(patientDTO.Name),
                Email = new Email(patientDTO.Email),
                Phone = new Phone(patientDTO.Phone),
                Conditions = patientDTO.Conditions.Select(condition => new Condition(condition)).ToList(), 
                EmergencyContact = new Phone(patientDTO.EmergencyContact),
                Address = new Address(address[0], address[1], address[2]),
                DateOfBirth = DateTime.Parse(patientDTO.DateOfBirth),
            };

            await _repo.AddAsync(patient);
            await _unitOfWork.CommitAsync();

            return ConvertToDTO(patient);
        }

        public async Task<PatientDTO> GetPatientByMedicalRecordNumber(PatientID id)
        {
            var patient = await _repo.GetPatientById(id);
            return patient == null ? null : ConvertToDTO(patient);
        }

        public async Task<PatientDTO> GetPatientByEmail(string email)
        {
            var patient = await _repo.GetPatientByEmail(email);
            return patient == null ? null : ConvertToDTO(patient);
        }

        public async Task<List<PatientDTO>> GetAllPatients()
        {
            var list = await _repo.GetAllPatients();  

            if (list == null)
            {
                return null;
            }

            
            List<PatientDTO> listDto = list.ConvertAll(pat => ConvertToDTO(pat));

            return listDto;  
        }

        private PatientDTO ConvertToDTO(Patient patient)
        {
            return new PatientDTO
            {
                MedicalRecordNumber = patient.Id.Value,
                Name = patient.Name.ToString(),
                Email = patient.Email.ToString(),
                Phone = patient.Phone.ToString(), 
                Conditions = patient.Conditions.Select(condition => condition.ToString()).ToList(), 
                EmergencyContact = patient.EmergencyContact.ToString(),
                Address = patient.Address.ToString(),
                DateOfBirth = patient.DateOfBirth.ToString("dd-MM-yyyy")
            };
        }
    }
}

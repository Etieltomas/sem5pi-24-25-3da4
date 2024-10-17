
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sempi5.Domain.Patient;
using Sempi5.Infrastructure.Databases;
using Sempi5.Infrastructure.Shared;

namespace Sempi5.Infrastructure.PatientRepository
{
    public class PatientRepository : BaseRepository<Patient,PatientID>, IPatientRepository
    {
        private readonly DataBaseContext _context;
        public PatientRepository(DataBaseContext context) : base(context.Patients)
        {
            _context = context;
        }
        
        public async Task<PatientDTO> AddPatient(PatientDTO patientDTO)
        {
        
            var users = await _context.Users.ToListAsync();
            var neededUser = users.FirstOrDefault(x => x.Email == patientDTO.Email);
           
            var patient = new Patient
            {
                Id = new PatientID(patientDTO.MedicalRecordNumber),
                SystemUser = neededUser,
                Name = patientDTO.Name,
                Email = patientDTO.Email,
                Phone = patientDTO.Phone,
                Conditions = patientDTO.Conditions,
                EmergencyContact = patientDTO.EmergencyContact,
                DateOfBirth = patientDTO.DateOfBirth
            };

            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();       
            
            return new PatientDTO {MedicalRecordNumber = patient.Id.Value, Name = patient.Name, Email = patient.Email, Phone = patient.Phone, Conditions = patient.Conditions, EmergencyContact = patient.EmergencyContact, DateOfBirth = patient.DateOfBirth };
        }

        public async Task<Patient> GetPatientByEmail(string email)
        {
            var patient = await  _context.Patients.FirstOrDefaultAsync(x => x.Email == email);

            if(patient == null){
                return null;
            }

            return patient;
        }

    }
}
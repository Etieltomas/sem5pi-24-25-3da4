
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sempi5.Domain.Patient;
using Sempi5.Infrastructure.Databases;
using Sempi5.Infrastructure.Shared;

namespace Sempi5.Infrastructure.PatientRepository
{
    public class PatientRepository : IPatientRepository
    {
        private readonly DataBaseContext _context;
        public PatientRepository(DataBaseContext context)
        {
            _context = context;
        }

        public async Task<PatientDTO> AddPatient(PatientDTO PatientDTO)
        {
            var patient = new Patient
            {
                MedicalRecordNumber = PatientDTO.MedicalRecordNumber,
                Name = PatientDTO.Name,
                Email = PatientDTO.Email,
                Phone = PatientDTO.Phone,
                Conditions = PatientDTO.Conditions,
                EmergencyContact = PatientDTO.EmergencyContact,
                DateOfBirth = PatientDTO.DateOfBirth
            };

            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();


            return new PatientDTO { MedicalRecordNumber = patient.MedicalRecordNumber, Name = patient.Name, Email = patient.Email, Phone = patient.Phone, Conditions = patient.Conditions, EmergencyContact = patient.EmergencyContact, DateOfBirth = patient.DateOfBirth };
        }

        public async Task<PatientDTO> GetPatientByMedicalRecordNumber(long id)
        {
            var patient = await  _context.Patients.FindAsync(id);

            if(patient == null){
                return null;
            }

            return new PatientDTO { MedicalRecordNumber = patient.MedicalRecordNumber, Name = patient.Name, Email = patient.Email, Phone = patient.Phone, Conditions = patient.Conditions, EmergencyContact = patient.EmergencyContact, DateOfBirth = patient.DateOfBirth };
        }

        public async Task<PatientDTO> GetPatientByEmail(string email)
        {
            var patient = await  _context.Patients.FirstOrDefaultAsync(x => x.Email == email);

            if(patient == null){
                return null;
            }

            return new PatientDTO { MedicalRecordNumber = patient.MedicalRecordNumber, Name = patient.Name, Email = patient.Email, Phone = patient.Phone, Conditions = patient.Conditions, EmergencyContact = patient.EmergencyContact, DateOfBirth = patient.DateOfBirth };
        }

        public async Task<ActionResult<IEnumerable<PatientDTO>>> GetAllPatients()
        {
            var list = await  _context.Patients.ToListAsync();
            
            List<PatientDTO> listDto = list.ConvertAll<PatientDTO>(cat => new PatientDTO { MedicalRecordNumber = cat.MedicalRecordNumber, Name = cat.Name, Email = cat.Email, Phone = cat.Phone, Conditions = cat.Conditions, EmergencyContact = cat.EmergencyContact, DateOfBirth = cat.DateOfBirth });

            return listDto;
        }

    }
}
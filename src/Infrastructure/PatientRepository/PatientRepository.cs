using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sempi5.Domain.Patient;
using Sempi5.Infrastructure.Databases;
using Sempi5.Infrastructure.Shared;

namespace Sempi5.Infrastructure.PatientRepository
{
    public class PatientRepository : BaseRepository<Patient, PatientID>, IPatientRepository
    {
        private readonly DataBaseContext _context;

        public PatientRepository(DataBaseContext context) : base(context.Patients)
        {
            _context = context;
        }

        public async Task<PatientDTO> AddPatient(PatientDTO patientDTO)
        {
            var users = await _context.Users.ToListAsync();
            //var neededUser = users.FirstOrDefault(x => x.Email.ToString().Equals(patientDTO.Email.ToString())); 

            var patient = new Patient
            {
                Id = new PatientID(patientDTO.MedicalRecordNumber),
                //SystemUser = neededUser,
                Name = new Name(patientDTO.Name), 
                Email = new Email(patientDTO.Email), 
                Phone = new Phone(patientDTO.Phone),
                Conditions = patientDTO.Conditions.Select(condition => new Condition(condition)).ToList(),
                EmergencyContact = new Phone(patientDTO.EmergencyContact),
                DateOfBirth = DateTime.Parse(patientDTO.DateOfBirth),
                Gender = GenderExtensions.FromString(patientDTO.Gender) 
            };

            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();

            return new PatientDTO
            {
                MedicalRecordNumber = patient.Id.Value,
                Name = patient.Name.ToString(),
                Email = patient.Email.ToString(),
                Phone = patient.Phone.ToString(),
                Conditions = patient.Conditions.Select(condition => condition.ToString()).ToList(),
                EmergencyContact = patient.EmergencyContact.ToString(),
                DateOfBirth = patient.DateOfBirth.ToString("dd-MM-yyyy"),
                Gender = patient.Gender.ToString()
            };
        }

        public async Task<Patient> GetPatientByEmail(string email)
        {
            var patient = await _context.Patients.FirstOrDefaultAsync(x => x.Email.ToString() == email);
            return patient;
        }
    }
}

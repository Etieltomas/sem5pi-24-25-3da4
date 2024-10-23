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

        public async Task<Patient> GetPatientByEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return null;
            }

            var patient = await _context.Patients
                .Include(p => p.SystemUser) 
                .FirstOrDefaultAsync(p => p.SystemUser == null || p.SystemUser.Email.Equals(new Email(email)));
                    
            return patient; 
        }

        public async Task<List<Patient>> GetAllPatients()
        {
            return await _context.Patients
                .Include(p => p.SystemUser)
                .ToListAsync();
        }

        public async Task<Patient> GetPatientById(PatientID id)
        {
            if (id == null)
            {
                return null;
            }

            var pat = await _context.Patients
                .Include(p => p.SystemUser)
                .FirstOrDefaultAsync(p => p.SystemUser == null ||  p.Id.AsString().Equals(id.ToString()));

            return pat; 
        }
    }

}

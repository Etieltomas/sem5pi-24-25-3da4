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
            var patient = await _context.Patients.FirstOrDefaultAsync(x => x.Email.ToString() == email);
            return patient;
        }
    }
}

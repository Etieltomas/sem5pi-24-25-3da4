using Microsoft.EntityFrameworkCore;
using Sempi5.Domain.PatientEntity;
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
                .FirstOrDefaultAsync(p => p.Email.Equals(new Email(email)));
                    
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
                .FirstOrDefaultAsync(p => p.Id.Equals(id));

            return pat; 
        }

        public async Task<Patient> GetPatientByName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return null;
            }

            var patient = await _context.Patients
                .Include(p => p.SystemUser) 
                .FirstOrDefaultAsync(p => p.SystemUser == null || p.Name.Equals(new Name(name)));
                    
            return patient; 
        }
        
        public async Task<List<Patient>> GetPatientsFiltered(string? name, string? email, string? dateOfBirth, string? medicalRecordNumber, int page, int pageSize)
        {
            var query = _context.Patients
                .Include(p => p.SystemUser)
                .AsQueryable();

            if (!string.IsNullOrEmpty(name)){
                query = query.Where(p => p.Name.Equals(new Name(name)));
            }	
            if (!string.IsNullOrEmpty(email)){
                query = query.Where(p => p.Email.Equals(new Email(email)));
            }
            if (!string.IsNullOrEmpty(dateOfBirth)){
                query = query.Where(p => p.DateOfBirth.Equals(DateTime.Parse(dateOfBirth)));
            }
            if (!string.IsNullOrEmpty(medicalRecordNumber)){
                query = query.Where(p => p.Id.Equals(new PatientID(medicalRecordNumber)));
            }

            query = query.Skip((page - 1) * pageSize).Take(pageSize);

            return await query.ToListAsync();
        }

        public async Task<List<Patient>> GetPatientsForDeletion(DateTime currentDateTime)
        {
            return await _context.Patients
                .Where(p => p.DeletePatientDate.HasValue && 
                p.DeletePatientDate.Value <= currentDateTime &&
                p.Email != new Email("anonymous@anonymous.anonymous"))
                .Include(p => p.SystemUser)
                .ToListAsync();
        }
    }

}

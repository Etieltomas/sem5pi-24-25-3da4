
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sempi5.Domain.SpecializationEntity;
using Sempi5.Domain.StaffEntity;
using Sempi5.Infrastructure.Databases;
using Sempi5.Infrastructure.Shared;

namespace Sempi5.Infrastructure.StaffRepository
{
    public class StaffRepository : BaseRepository<Staff, StaffID>, IStaffRepository
    {
        private readonly DataBaseContext _context;

        public StaffRepository(DataBaseContext context) : base(context.Staff)
        {
            _context = context;
        }

        public async Task<Staff> GetStaffMemberByEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return null;
            }

            var staff = await _context.Staff
                .Include(s => s.SystemUser)
                .Include(s => s.Specialization)
                .FirstOrDefaultAsync(s => s.Email.Equals(new Email(email)));
                    
            return staff;
        }

        public async Task<List<Staff>> GetAllStaffMembers()
        {
            return await _context.Staff
                .Include(s => s.SystemUser)
                .Include(s => s.Specialization)
                .ToListAsync();
        }

        public async Task<Staff> GetStaffMemberById(StaffID id)
        {
            if (id == null)
            {
                return null;
            }

            var staff = await _context.Staff
                .Include(s => s.SystemUser)
                .Include(s => s.Specialization)
                .FirstOrDefaultAsync(s => s.Id.Equals(id)); 
            
            return staff;
        }

        public async Task<List<Staff>> SearchStaff(string? name, string? email, string? specialization,
                             int page, int pageSize)
        {
            var query = _context.Staff
                .Include(s => s.SystemUser)
                .Include(s => s.Specialization)
                .AsQueryable();

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(s => s.Name.Equals(new Name(name)));
            }

            if (!string.IsNullOrEmpty(email))
            {
                query = query.Where(s => s.Email.Equals(new Email(email)));
            }

            if (!string.IsNullOrEmpty(specialization))
            {
                query = query.Where(s => s.Specialization.Id.Equals(new SpecializationID(specialization)));
            }

            query = query.Skip((page - 1) * pageSize).Take(pageSize);


            return await query.ToListAsync();
        }
    }

}
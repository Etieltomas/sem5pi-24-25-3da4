
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
                .FirstOrDefaultAsync(s => s.Specialization == null || s.Email.Equals(new Email(email)));
                    
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
                .FirstOrDefaultAsync(s => s.SystemUser == null || s.Id.Equals(id)); 
            
            return staff;
        }
    }

}
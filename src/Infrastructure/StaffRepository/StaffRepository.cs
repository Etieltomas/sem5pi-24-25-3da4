
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sempi5.Domain.Staff;
using Sempi5.Infrastructure.Databases;
using Sempi5.Infrastructure.Shared;

namespace Sempi5.Infrastructure.StaffRepository
{
    public class StaffRepository : BaseRepository<Staff,StaffID>, IStaffRepository
    {
        private readonly DataBaseContext _context;

        public StaffRepository(DataBaseContext context) : base(context.Staff)
        {
            _context = context;
        }

        public async Task<Staff> GetStaffMemberByEmail(string email)
        {
            var staff = await _context.Staff.FirstOrDefaultAsync(x => x.Email.ToString() == email);

            if (staff == null)
            {
                return null;
            }

            return staff;
        }


        
    }
}
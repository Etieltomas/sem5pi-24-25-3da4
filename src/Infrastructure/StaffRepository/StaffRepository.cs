
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sempi5.Domain.Staff;
using Sempi5.Infrastructure.Databases;
using Sempi5.Infrastructure.Shared;

namespace Sempi5.Infrastructure.StaffRepository
{
    public class StaffRepository : IStaffRepository
    {
        private readonly DataBaseContext _context;

        public StaffRepository(DataBaseContext context)
        {
            _context = context;
        }

        public async Task<StaffDTO> AddStaffMember(StaffDTO staffDTO)
        {
            var staff = new Staff
            {
                LicenseNumber = staffDTO.LicenseNumber,
                Name = staffDTO.Name,
                Email = staffDTO.Email,
                Phone = staffDTO.Phone,
                AvailabilitySlots = staffDTO.AvailabilitySlots,
                Specialization = staffDTO.Specialization
            };

            _context.Staff.Add(staff);
            await _context.SaveChangesAsync();

            return new StaffDTO { LicenseNumber = staff.LicenseNumber, Name = staff.Name, Email = staff.Email, Phone = staff.Phone, AvailabilitySlots = staff.AvailabilitySlots, Specialization = staff.Specialization };
        }

        public async Task<Staff> GetStaffMember(long id)
        { 
            var staff = await  _context.Staff.FindAsync(id);

            if(staff == null)
                return null;

            return staff;
        }

        public async Task<Staff> GetStaffMemberByEmail(string email)
        {
            var staff = await  _context.Staff.FirstOrDefaultAsync(x => x.Email == email);

            if(staff == null){
                return null;
            }

            return staff;
        }

        public async Task<ActionResult<IEnumerable<Staff>>> GetAllStaffMembers()
        {
            var list = await  _context.Staff.ToListAsync();
            
            return list;
        }

        
    }
}
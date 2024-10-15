
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


        public async Task<StaffDTO> AddStaffMember(StaffDTO staffDTO)
        {
            var users = await _context.Users.ToListAsync();
            var neededUser = users.FirstOrDefault(x => x.Email == staffDTO.Email);

            var staff = new Staff
            {
                Name = staffDTO.Name,
                Email = staffDTO.Email,
                Phone = staffDTO.Phone,
                AvailabilitySlots = staffDTO.AvailabilitySlots,
                Specialization = staffDTO.Specialization,
                SystemUser = neededUser
            };

            _context.Staff.Add(staff);
            await _context.SaveChangesAsync();

            return new StaffDTO { LicenseNumber = staff.Id.Value, Name = staff.Name, Email = staff.Email, Phone = staff.Phone, AvailabilitySlots = staff.AvailabilitySlots, Specialization = staff.Specialization };
        }

        public async Task<Staff> GetStaffMemberByEmail(string email)
        {
            var staff = await  _context.Staff.FirstOrDefaultAsync(x => x.Email == email);

            if(staff == null){
                return null;
            }

            return staff;
        }

        
    }
}
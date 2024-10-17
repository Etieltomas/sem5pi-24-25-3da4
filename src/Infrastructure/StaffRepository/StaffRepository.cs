
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

            var availabilitySlots = staffDTO.AvailabilitySlots.Select(slot => new AvailabilitySlot(slot)).ToList();

            var staff = new Staff
            {
                Name = new Name(staffDTO.Name),
                Email = new Email(staffDTO.Email),
                Phone = new Phone(staffDTO.Phone),
                AvailabilitySlots = availabilitySlots, 
                Specialization = staffDTO.Specialization,
                SystemUser = neededUser
            };

            _context.Staff.Add(staff);
            await _context.SaveChangesAsync();

            return new StaffDTO 
            { 
                Id = staff.Id.Value,
                LicenseNumber = staff.LicenseNumber.ToString(), 
                Name = staff.Name.ToString(), 
                Email = staff.Email.ToString(), 
                Phone = staff.Phone.ToString(), 
                AvailabilitySlots = staff.AvailabilitySlots.Select(slot => slot.ToString()).ToList(), // Convert to List<string>
                Specialization = staff.Specialization 
            };
        }


        public async Task<Staff> GetStaffMemberByEmail(string email)
        {
            var staff = await _context.Staff.FirstOrDefaultAsync(x => x.Email.ToString() == email); // Adjust to access Email.Value

            if (staff == null)
            {
                return null;
            }

            return staff;
        }


        
    }
}
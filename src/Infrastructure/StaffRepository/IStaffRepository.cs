
using Microsoft.AspNetCore.Mvc;
using Sempi5;
using Sempi5.Domain;
using Sempi5.Domain.Shared;
using Sempi5.Domain.Staff;

namespace Sempi5.Infrastructure.StaffRepository
{
    public interface IStaffRepository
    {
        public Task<StaffDTO> AddStaffMember(StaffDTO staffDTO);
        public Task<Staff> GetStaffMember(long id);
        public Task<Staff> GetStaffMemberByEmail(string email);
        public Task<ActionResult<IEnumerable<Staff>>> GetAllStaffMembers();
        
    }
}
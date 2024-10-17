
using Microsoft.AspNetCore.Mvc;
using Sempi5;
using Sempi5.Domain;
using Sempi5.Domain.Shared;
using Sempi5.Domain.Staff;

namespace Sempi5.Domain.Staff
{
    public interface IStaffRepository : IRepository<Staff, StaffID>
    {
        public Task<Staff> GetStaffMemberByEmail(string email);
        public Task<StaffDTO> AddStaffMember(StaffDTO staffDTO);
    }
}
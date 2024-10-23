using Sempi5.Domain.Shared;

namespace Sempi5.Domain.StaffEntity
{
    public interface IStaffRepository : IRepository<Staff, StaffID>
    {
        public Task<Staff> GetStaffMemberByEmail(string email);
        public Task<List<Staff>> GetAllStaffMembers();
        public Task<Staff> GetStaffMemberById(StaffID id);
    }
}
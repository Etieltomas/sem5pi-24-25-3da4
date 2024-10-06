using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sempi5.Domain.Shared;
using Sempi5.Infrastructure.Databases;
using Sempi5.Infrastructure.StaffRepository;

namespace Sempi5.Domain.Staff
{
    public class StaffService
    {
        private readonly IStaffRepository _repo;

        public StaffService(IStaffRepository repo)
        {
            this._repo = repo;
        }

        public Task<StaffDTO> AddStaffMember(StaffDTO staffDTO){
            return _repo.AddStaffMember(staffDTO);
        }

        public Task<StaffDTO> GetStaffMember(long id){
            return _repo.GetStaffMember(id);
        }
     
        public Task<ActionResult<IEnumerable<StaffDTO>>> GetAllStaffMembers(){
            return _repo.GetAllStaffMembers();
        }
    }
}
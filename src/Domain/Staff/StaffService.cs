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

        public async Task<StaffDTO> AddStaffMember(StaffDTO staffDTO){
            return await _repo.AddStaffMember(staffDTO);
        }

        public async Task<StaffDTO> GetStaffMember(long id){

            var staff = await _repo.GetStaffMember(id);

            return new StaffDTO { LicenseNumber = staff.LicenseNumber, Name = staff.Name, Email = staff.Email, Phone = staff.Phone, AvailabilitySlots = staff.AvailabilitySlots, Specialization = staff.Specialization };      
        }
    
        public async Task<ActionResult<IEnumerable<StaffDTO>>> GetAllStaffMembers(){
            var result = await _repo.GetAllStaffMembers();

            if (result == null)
            {
                return null;
            }

            var list = result.Value.ToList();

            List<StaffDTO> listDto = list.ConvertAll<StaffDTO>(staff => new StaffDTO { LicenseNumber = staff.LicenseNumber, Name = staff.Name, Email = staff.Email, Phone = staff.Phone, AvailabilitySlots = staff.AvailabilitySlots, Specialization = staff.Specialization });
            
            return listDto;
        }
    }
}